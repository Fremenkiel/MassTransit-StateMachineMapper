using System;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.DependencyInjection.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace StateMachineMapper.StateMachine.TransientRegistrationConfigurator;

public class TransientRegistrationConfigurator : RegistrationConfigurator
{
    readonly IServiceCollection _collection;
    ISagaRepositoryRegistrationProvider _sagaRepositoryRegistrationProvider;

    protected TransientRegistrationConfigurator(IServiceCollection collection, IContainerRegistrar registrar) : base(collection, registrar)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        _sagaRepositoryRegistrationProvider = new SagaRepositoryRegistrationProvider();
    }

    public ISagaRegistrationConfigurator<T> AddTransientSagaStateMachine<TStateMachine, T>(Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
        where TStateMachine : class, SagaStateMachine<T>
        where T : class, SagaStateMachineInstance
    {
        return AddTransientSagaStateMachine<TStateMachine, T>(null, configure);
    }

    public ISagaRegistrationConfigurator<T> AddTransientSagaStateMachine<TStateMachine, T>(Type sagaDefinitionType,
        Action<IRegistrationContext, ISagaConfigurator<T>> configure = null)
        where TStateMachine : class, SagaStateMachine<T>
        where T : class, SagaStateMachineInstance
    {
        var registration = _collection.RegisterSagaStateMachine<TStateMachine, T>(Registrar, sagaDefinitionType);

        registration.AddConfigureAction(configure);

        return new SagaRegistrationConfigurator<T>(this, registration);
    }


}
