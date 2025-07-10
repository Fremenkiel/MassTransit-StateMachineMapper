using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.StateMachine.Data;
using MassTransit;

namespace StateMachineMapper.StateMachine.Factories.Interfaces;

public interface IMessageFactory
{
    EventActivityBinder<OnboardingStateMachineData, TMessage> Apply<TMessage>(EventActivityBinder<OnboardingStateMachineData, TMessage> binder)
        where TMessage : class, ICorrelatableById;

    BehaviorContext<OnboardingStateMachineData, TMessage> Apply<TMessage>(BehaviorContext<OnboardingStateMachineData, TMessage> context)
        where TMessage : class, ICorrelatableById;
}
