using System;
using System.Collections.Generic;
using System.Linq;
using StateMachineMapper.Commands;
using StateMachineMapper.Entities;
using StateMachineMapper.Events;
using StateMachineMapper.Sagas.Builder;
using StateMachineMapper.Sagas.Correlator;
using StateMachineMapper.Sagas.Data;
using StateMachineMapper.Sagas.Factories;
using StateMachineMapper.Sagas.Factories.Interfaces;
using StateMachineMapper.Sagas.Wrapper;
using MassTransit;
using StateMachineMapper.Behavior;

namespace StateMachineMapper.StateMachine;

public class OnboardingSaga : CorrelatingSaga<OnboardingSagaData>
{
    public required State Welcoming { get; set; }
    public required State FollowingUp { get; set; }
    public required State Onboarding { get; set; }
    
    public required Event<SubscriberCreated> SubscriberCreated { get; set; }
    public required Event<WelcomeEmailSent> WelcomeEmailSent { get; set; }
    public required Event<FollowUpEmailSent> FollowUpEmailSent { get; set; }
    
    private readonly Dictionary<string, Action<OnboardingSaga>> _eventConfigurators;
    private readonly Dictionary<string, IEventBehaviorBuilder> _eventBuilders;

    internal readonly Dictionary<string, ActivityWrapper> ThenActivities;
    internal readonly Dictionary<string, IMessageFactory> PublishFactories;

    public OnboardingSaga()
    {
        _eventConfigurators = new()
        {
            ["SubscriberCreated"] = saga => CorrelateEventById(() => saga.SubscriberCreated),
            ["WelcomeEmailSent"] = saga => CorrelateEventById(() => saga.WelcomeEmailSent),
            ["FollowUpEmailSent"] = saga => CorrelateEventById(() => saga.FollowUpEmailSent)
        };

        ThenActivities = new()
        {
            ["SetSubscriptionInfo"] = new ActivityWrapper(context =>
            {
                if (context.Message is SubscriberCreated msg)
                {
                    context.Saga.SubscriptionId = msg.SubscriptionId;
                    context.Saga.Email = msg.Email;
                }
            }),
            ["MarkWelcomeEmailSent"] = new ActivityWrapper(context => context.Saga.WelcomeEmailSent = true),
            ["MarkFollowUpEmailSentAndComplete"] = new ActivityWrapper(context =>
            {
                context.Saga.FollowUpEmailSent = true;
                context.Saga.OnboardingCompleted = true;
            }),
        };

        PublishFactories = new()
        {
            ["SendWelcomeEmail"] = new MessageFactory<SendWelcomeEmail>(context => new SendWelcomeEmail(context.Message.SubscriptionId, context.Saga.Email)),
            ["SendFollowUpEmail"] = new MessageFactory<SendFollowUpEmail>(context => new SendFollowUpEmail(context.Message.SubscriptionId, context.Saga.Email)),
            ["PublishOnboardingCompleted"] = new MessageFactory<OnboardingCompleted>(context => new OnboardingCompleted { SubscriptionId = context.Message.SubscriptionId }),
        };

        _eventBuilders = new Dictionary<string, IEventBehaviorBuilder>
        {
            [nameof(SubscriberCreated)] = new EventBehaviorBuilder<SubscriberCreated>(SubscriberCreated!),
            [nameof(WelcomeEmailSent)] = new EventBehaviorBuilder<WelcomeEmailSent>(WelcomeEmailSent!),
            [nameof(FollowUpEmailSent)] = new EventBehaviorBuilder<FollowUpEmailSent>(FollowUpEmailSent!),
        };
        
        
        MapEvents(_eventConfigurators.Select(x => x.Key).Distinct().ToList(), this);
        
        MapSagas(new List<StateMachineTemplates>()
        {
           BehaviorDefiner.GetBehavior()
        });
    }
    
    public void MapSagas(List<StateMachineTemplates> behaviorSteps)
    {
        BehaviorBuilder(behaviorSteps);
    }

    private void MapEvents(List<string> eventList, OnboardingSaga onboardingSaga)
    {
        foreach (var @event in eventList)
        {
            if (_eventConfigurators.TryGetValue(@event, out Action<OnboardingSaga> configureAction))
            {
                configureAction(onboardingSaga);
            }
        }
    }

    private void BehaviorBuilder(List<StateMachineTemplates> behaviorStepsList)
    {
        InstanceState(x => x.CurrentState);
        
        foreach (var behaviorSteps in behaviorStepsList)
        {
            var behaviors = behaviorSteps.Entries.GroupBy(s => s.TriggerEventName);

            foreach (var behaviorGroup in behaviors)
            {
                if (_eventBuilders.TryGetValue(behaviorGroup.Key, out var builder))
                {
                    builder.Build(this, behaviorGroup);
                }
            }
        }

        SetCompletedWhenFinalized();
    }

    
}