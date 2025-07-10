using System;
using System.Threading.Tasks;
using MassTransit;

namespace StateMachineMapper.StateMachine.Manager.Interfaces;

public interface IDynamicStateMachineManager
{
    Task ConnectStateMachine<TState>(Guid queueName) where TState : class, SagaStateMachineInstance, new();
    Task DisconnectStateMachine(Guid queueName);
}
