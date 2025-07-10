using System;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.StateMachine.Factories.Interfaces;
using MassTransit;

namespace StateMachineMapper.StateMachine.Factories;

public class MessageFactory<TMessageToPublish> : IMessageFactory
    where TMessageToPublish : class
{
    private readonly Func<BehaviorContext<OnboardingStateMachineData, ICorrelatableById>, TMessageToPublish> _factory;

    public MessageFactory(Func<BehaviorContext<OnboardingStateMachineData, ICorrelatableById>, TMessageToPublish> factory)
    {
        _factory = factory;
    }

    public EventActivityBinder<OnboardingStateMachineData, TMessage> Apply<TMessage>(EventActivityBinder<OnboardingStateMachineData, TMessage> binder)
        where TMessage : class, ICorrelatableById
    {
        binder.Publish(TypedFactory);
        return binder;

        TMessageToPublish TypedFactory(BehaviorContext<OnboardingStateMachineData, TMessage> context)
        {
            return _factory(context);
        }
    }

    public BehaviorContext<OnboardingStateMachineData, TMessage> Apply<TMessage>(BehaviorContext<OnboardingStateMachineData, TMessage> context)
        where TMessage : class, ICorrelatableById
    {
        context.Publish(_factory(context));
        return context;
    }
}
