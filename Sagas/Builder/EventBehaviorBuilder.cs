using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using StateMachineMapper.Entities;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.Sagas.Data;
using StateMachineMapper.StateMachine;

namespace StateMachineMapper.Sagas.Builder;


public interface IEventBehaviorBuilder
{
    void Build(OnboardingSaga saga, IGrouping<string,SagStateMachineTemplateEntry> behaviorGroup);
}

public class EventBehaviorBuilder<TMessage> : IEventBehaviorBuilder
    where TMessage : class, ICorrelatableById
{
    private readonly Event<TMessage> _event;

    public EventBehaviorBuilder(Event<TMessage> @event)
    {
        _event = @event;
    }

    public void Build(OnboardingSaga saga, IGrouping<string,SagStateMachineTemplateEntry> behaviorGroup)
    {
            
            var initialStateName = behaviorGroup.FirstOrDefault(x => x.InitialStateName != "")?.InitialStateName;

            if (initialStateName == null)
                return;

            if (initialStateName == "Initially")
            {
                    if (saga.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                    {
                        saga.InitiallyBinder(ApplyActivities(saga, behaviorGroup)
                            .TransitionTo(saga.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                            .Then(context => factory.Apply(context)));
                    }
            }
            else if (behaviorGroup.Any(x => x.ActionType == "Finalize"))
            {
                if (saga.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                {
                    saga.DuringBinder(saga.StateBinder(initialStateName),ApplyActivities(saga, behaviorGroup)
                        .TransitionTo(saga.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                        .Then(context => factory.Apply(context))
                        .Finalize());
                }
            }
            else
            {
                if (saga.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                {
                    saga.DuringBinder(saga.StateBinder(initialStateName),ApplyActivities(saga, behaviorGroup)
                        .TransitionTo(saga.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                        .Then(context => factory.Apply(context)));
                }
            }
    }


    private EventActivityBinder<OnboardingSagaData, TMessage> ApplyActivities(OnboardingSaga saga, IEnumerable<SagStateMachineTemplateEntry> steps)
    {
        var binder = saga.CreateBinder(_event);
        foreach (var step in steps)
        {
            switch (step.ActionType)
            {
                case "Then":
                    if (saga.ThenActivities.TryGetValue(step.ActionParameter, out var thenWrapper))
                    {
                        Action<BehaviorContext<OnboardingSagaData, TMessage>> action = context =>
                        {
                            thenWrapper.Logic(context);
                        };

                        binder.Then(action);
                    }
                    break;
            }
        }
        return binder;
    }
}
