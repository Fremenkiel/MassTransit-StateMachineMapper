using System;
using System.Collections.Generic;
using System.Linq;
using StateMachineMapper.Commands;
using StateMachineMapper.Entities;
using StateMachineMapper.Events;
using StateMachineMapper.StateMachine.Builder;
using StateMachineMapper.StateMachine.Correlator;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.StateMachine.Factories;
using StateMachineMapper.StateMachine.Factories.Interfaces;
using StateMachineMapper.StateMachine.Wrapper;
using MassTransit;
using StateMachineMapper.StateMachine.Builder.Interfaces;
using StateMachineMapper.StateMachine.Manager;

namespace StateMachineMapper.StateMachine;

public class OnboardingStateMachine : CorrelatingStateMachine<OnboardingStateMachineData>
{
    public required State Welcoming { get; set; }
    public required State FollowingUp { get; set; }
    public required State Onboarding { get; set; }

    public required Event<SubscriberCreated> SubscriberCreated { get; set; }
    public required Event<WelcomeEmailSent> WelcomeEmailSent { get; set; }
    public required Event<FollowUpEmailSent> FollowUpEmailSent { get; set; }

    private readonly Dictionary<string, Action<OnboardingStateMachine>> _eventConfigurators;
    private readonly Dictionary<string, IEventBehaviorBuilder> _eventBuilders;

    internal readonly Dictionary<string, ActivityWrapper> ThenActivities;
    internal readonly Dictionary<string, IMessageFactory> PublishFactories;

    public OnboardingStateMachine(EndpointManager endpointManager)
    {
        StateMachineTemplate behaviorSteps = endpointManager.GetStateMachineTemplate();

        _eventConfigurators = new Dictionary<string, Action<OnboardingStateMachine>>
        {
            ["SubscriberCreated"] = saga => CorrelateEventById(() => saga.SubscriberCreated),
            ["WelcomeEmailSent"] = saga => CorrelateEventById(() => saga.WelcomeEmailSent),
            ["FollowUpEmailSent"] = saga => CorrelateEventById(() => saga.FollowUpEmailSent)
        };

        ThenActivities = new Dictionary<string, ActivityWrapper>
        {
            ["SetSubscriptionInfo"] = new (context =>
            {
                if (context.Message is SubscriberCreated msg)
                {
                    context.Saga.SubscriptionId = msg.SubscriptionId;
                    context.Saga.Email = msg.Email;
                }
            }),
            ["MarkWelcomeEmailSent"] = new (context => context.Saga.WelcomeEmailSent = true),
            ["MarkFollowUpEmailSentAndComplete"] = new (context =>
            {
                context.Saga.FollowUpEmailSent = true;
                context.Saga.OnboardingCompleted = true;
            }),
        };

        PublishFactories = new Dictionary<string, IMessageFactory>
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

        BehaviorBuilder(behaviorSteps);
    }
    private void MapEvents(List<string> eventList, OnboardingStateMachine onboardingStateMachine)
    {
        foreach (var @event in eventList)
        {
            if (_eventConfigurators.TryGetValue(@event, out Action<OnboardingStateMachine> configureAction))
            {
                configureAction(onboardingStateMachine);
            }
        }
    }

    private void BehaviorBuilder(StateMachineTemplate behaviorSteps)
    {
        InstanceState(x => x.CurrentState);

        var behaviors = behaviorSteps.Entries.GroupBy(s => s.TriggerEventName);

        foreach (var behaviorGroup in behaviors)
        {
            if (_eventBuilders.TryGetValue(behaviorGroup.Key, out var builder))
            {
                builder.Build(this, behaviorGroup);
            }
        }

        SetCompletedWhenFinalized();
    }
}
