using System;
using MassTransit;

namespace StateMachineMapper.StateMachine.TransientRegistrationConfigurator.Interfaces;

public interface ITransientRegistrationConfigurator : IRegistrationConfigurator
{
    ISagaRegistrationConfigurator<T> AddTransientSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
        where TStateMachine : class, SagaStateMachine<T>
        where T : class, SagaStateMachineInstance;

    ISagaRegistrationConfigurator<T> AddTransientSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType,
        Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
        where TStateMachine : class, SagaStateMachine<T>
        where T : class, SagaStateMachineInstance;

}
