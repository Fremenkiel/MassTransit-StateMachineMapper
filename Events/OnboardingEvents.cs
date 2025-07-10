using System;
using StateMachineMapper.Events.Interfaces;

namespace StateMachineMapper.Events;

public record SubscriberCreated : ICorrelatableById
{
    public Guid CorrelationId { get; init; }
    public Guid SubscriptionId { get; set; }
    public string Email { get; set; }
}

public class WelcomeEmailSent : ICorrelatableById
{
    public Guid CorrelationId { get; init; }
    public Guid SubscriptionId { get; set; }
    public string Email { get; set; }
}

public class FollowUpEmailSent : ICorrelatableById
{
    public Guid CorrelationId { get; init; }
    public Guid SubscriptionId { get; set; }
    public string Email { get; set; }
}


public class OnboardingCompleted : ICorrelatableById
{
    public Guid CorrelationId { get; init; }
    public Guid SubscriptionId { get; set; }
}
