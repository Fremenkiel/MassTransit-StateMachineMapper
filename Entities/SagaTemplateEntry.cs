using System;

namespace StateMachineMapper.Entities;

public class SagStateMachineTemplateEntry
{
    public int Id { get; set; }
    public string InitialStateName { get; set; }
    public string TriggerEventName { get; set; }
    public string ActionType { get; set; }
    public string ActionParameter { get; set; }
    public Guid TemplateId { get; set; }
    public StateMachineTemplates Template { get; set; }
}
