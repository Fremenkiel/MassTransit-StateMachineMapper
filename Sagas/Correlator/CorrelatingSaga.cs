using System;
using System.Linq.Expressions;
using MassTransit;
using StateMachineMapper.Events.Interfaces;

namespace StateMachineMapper.Sagas.Correlator;



public abstract class CorrelatingSaga<TInstance> : MassTransitStateMachine<TInstance>
    where TInstance : class, SagaStateMachineInstance
{
    protected void CorrelateEventById<TMessage>(Expression<Func<Event<TMessage>>> propertyExpression)
        where TMessage : class, ICorrelatableById
    {
        Event(propertyExpression, x => x.CorrelateById(context => context.Message.SubscriptionId));
    }

    public EventActivityBinder<TInstance, TMessage> CreateBinder<TMessage>(Event<TMessage> @event) where TMessage : class
    {
        return When(@event);
    }

    public void InitiallyBinder(EventActivities<TInstance> eventActivities)
    {
        Initially(eventActivities);
    }

    public void DuringBinder(State state, EventActivities<TInstance> eventActivities)
    {
        During(state, eventActivities);
    }

    public State<TInstance> StateBinder(string name)
    {
        return State(name);
    }
}
