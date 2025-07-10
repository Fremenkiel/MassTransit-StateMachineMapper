using System;
using MassTransit;

namespace StateMachineMapper.Events.Interfaces;

public interface ICorrelatableById : CorrelatedBy<Guid>
{
    Guid SubscriptionId { get; }
}
