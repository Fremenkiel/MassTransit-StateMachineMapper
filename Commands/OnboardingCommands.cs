using System;

namespace StateMachineMapper.Commands;

public record Onboarding(Guid TemplateId, string Email);
public record SendWelcomeEmail(Guid TemplateId, Guid SubscriptionId, string Email);
public record SendFollowUpEmail(Guid TemplateId, Guid SubscriptionId, string Email);
