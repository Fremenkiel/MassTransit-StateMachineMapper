using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.Sagas.Data;
using MassTransit;

namespace StateMachineMapper.Sagas.Factories.Interfaces;

public interface IMessageFactory
{
    EventActivityBinder<OnboardingSagaData, TMessage> Apply<TMessage>(EventActivityBinder<OnboardingSagaData, TMessage> binder)
        where TMessage : class, ICorrelatableById;

    BehaviorContext<OnboardingSagaData, TMessage> Apply<TMessage>(BehaviorContext<OnboardingSagaData, TMessage> context)
        where TMessage : class, ICorrelatableById;
}