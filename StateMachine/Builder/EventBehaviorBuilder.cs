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

            if (initialStateName == "Initially")
            {
                    if (stateMachine.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                    {
                        stateMachine.InitiallyBinder(ApplyActivities(stateMachine, behaviorGroup)
                            .TransitionTo(stateMachine.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                            .Then(context => factory.Apply(context)));
                    }
            }
            else if (behaviorGroup.Any(x => x.ActionType == "Finalize"))
            {
                if (stateMachine.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                {
                    stateMachine.DuringBinder(stateMachine.StateBinder(initialStateName),ApplyActivities(stateMachine, behaviorGroup)
                        .TransitionTo(stateMachine.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                        .Then(context => factory.Apply(context))
                        .Finalize());
                }
            }
            else
            {
                if (stateMachine.PublishFactories.TryGetValue(behaviorGroup.FirstOrDefault(x => x.ActionType == "Publish")?.ActionParameter ?? "", out var factory))
                {
                    stateMachine.DuringBinder(stateMachine.StateBinder(initialStateName),ApplyActivities(stateMachine, behaviorGroup)
                        .TransitionTo(stateMachine.GetState(behaviorGroup.FirstOrDefault(x => x.ActionType == "TransitionTo")?.ActionParameter ?? ""))
                        .Then(context => factory.Apply(context)));
                }
            }
    }

    private EventActivityBinder<OnboardingStateMachineData, TMessage> ApplyActivities(OnboardingStateMachine stateMachine, IEnumerable<StateMachineTemplateEntry> steps)
    {
        var binder = stateMachine.CreateBinder(_event);
        foreach (var step in steps)
        {
            switch (step.ActionType)
            {
                case "Then":
                    if (stateMachine.ThenActivities.TryGetValue(step.ActionParameter, out var thenWrapper))
                    {
                        var wrapper = thenWrapper;

                        void Action(BehaviorContext<OnboardingStateMachineData, TMessage> context)
                        {
                            wrapper.Logic(context);
                        }

                        binder.Then(Action);
                    }
                    break;
            }
        }
        return binder;
    }
}
