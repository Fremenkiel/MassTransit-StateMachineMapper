using System;

namespace StateMachineMapper.Commands;

public record Onboarding(string Email);
public record SendWelcomeEmail(Guid SubscriptionId, string Email);
public record SendFollowUpEmail(Guid SubscriptionId, string Email);
