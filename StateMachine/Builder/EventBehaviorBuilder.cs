using System.Collections.Generic;
using System.Linq;
using MassTransit;
using StateMachineMapper.Entities;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.StateMachine.Builder.Interfaces;

namespace StateMachineMapper.StateMachine.Builder;

public class EventBehaviorBuilder<TMessage> : IEventBehaviorBuilder
    where TMessage : class, ICorrelatableById
{
    private readonly Event<TMessage> _event;

    public EventBehaviorBuilder(Event<TMessage> @event)
    {
        _event = @event;
    }

    public void Build(OnboardingStateMachine stateMachine, IGrouping<string,StateMachineTemplateEntry> behaviorGroup)
    {
        var initialStateName = behaviorGroup.FirstOrDefault(x => x.InitialStateName != "")?.InitialStateName;

        if (initialStateName == null)
            return;

        if (stateMachine.ThenActivities.TryGetValue(
                behaviorGroup.First(x => x.ActionType == "Then").ActionParameter, out var thenWrapper))
        {
            if (stateMachine.PublishFactories.TryGetValue(
                    behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "",
                    out var factory))
            {
                if (initialStateName == "Initially")
                {
                    stateMachine.InitiallyBinder(stateMachine.CreateBinder(_event)
                        .Then(Action)
                        .TransitionTo(stateMachine.GetState(behaviorGroup
                            .FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                        .Then(context => factory.Apply(context)));
                }
                else if (behaviorGroup.Any(x => x.ActionType == "Finalize"))
                {
                    stateMachine.DuringBinder(stateMachine.StateBinder(initialStateName),
                        stateMachine.CreateBinder(_event)
                            .Then(Action)
                            .TransitionTo(stateMachine.GetState(behaviorGroup
                                .FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                            .Then(context => factory.Apply(context))
                            .Finalize());
                }
                else
                {
                    stateMachine.DuringBinder(stateMachine.StateBinder(initialStateName),
                        stateMachine.CreateBinder(_event)
                            .Then(Action)
                            .TransitionTo(stateMachine.GetState(behaviorGroup
                                .FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                            .Then(context => factory.Apply(context)));
                }
            }
            void Action(BehaviorContext<OnboardingStateMachineData, TMessage> context)
            {
                thenWrapper.Logic(context);
            }
        }
    }
}
