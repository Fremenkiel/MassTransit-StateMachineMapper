using System;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.Sagas.Data;
using StateMachineMapper.Sagas.Factories.Interfaces;
using MassTransit;

namespace StateMachineMapper.Sagas.Factories;

public class MessageFactory<TMessageToPublish> : IMessageFactory
    where TMessageToPublish : class
{
    private readonly Func<BehaviorContext<OnboardingSagaData, ICorrelatableById>, TMessageToPublish> _factory;

    public MessageFactory(Func<BehaviorContext<OnboardingSagaData, ICorrelatableById>, TMessageToPublish> factory)
    {
        _factory = factory;
    }

    public EventActivityBinder<OnboardingSagaData, TMessage> Apply<TMessage>(EventActivityBinder<OnboardingSagaData, TMessage> binder)
        where TMessage : class, ICorrelatableById
    {
        EventMessageFactory<OnboardingSagaData, TMessage, TMessageToPublish> typedFactory = context =>
        {
            return _factory(context);
        };
        binder.Publish(typedFactory);
        return binder;
    }

    public BehaviorContext<OnboardingSagaData, TMessage> Apply<TMessage>(BehaviorContext<OnboardingSagaData, TMessage> context)
        where TMessage : class, ICorrelatableById
    {
        context.Publish(_factory(context));
        return context;
    }
}
