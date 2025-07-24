using System;
using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace StateMachineMapper.StateMachine.Data;

public class OnboardingStateMachineData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    [StringLength(50)]
    public string CurrentState { get; set; }

    public Guid SubscriptionId { get; set; }
    public Guid TemplateId { get; set; }
    [StringLength(500)]
    public string Email { get; set; } = string.Empty;
    public bool WelcomeEmailSent { get; set; }
    public bool FollowUpEmailSent { get; set; }
    public bool OnboardingCompleted { get; set; }
    public uint RowVersion { get; set; }
}
