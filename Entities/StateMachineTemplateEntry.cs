using System;
using System.ComponentModel.DataAnnotations;

namespace StateMachineMapper.Entities;

public class StateMachineTemplateEntry
{
    public int Id { get; set; }
    [StringLength(128)]
    public string InitialStateName { get; set; }
    [StringLength(128)]
    public string TriggerEventName { get; set; }
    [StringLength(128)]
    public string ActionType { get; set; }
    [StringLength(128)]
    public string ActionParameter { get; set; }
    public Guid TemplateId { get; set; }
    public StateMachineTemplate Template { get; set; }
}
