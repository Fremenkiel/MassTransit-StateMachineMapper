using System;
using MassTransit;

namespace StateMachineMapper.Sagas.Data;

public class OnboardingSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    
    public Guid SubscriptionId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool WelcomeEmailSent { get; set; }
    public bool FollowUpEmailSent { get; set; }
    public bool OnboardingCompleted { get; set; }
}