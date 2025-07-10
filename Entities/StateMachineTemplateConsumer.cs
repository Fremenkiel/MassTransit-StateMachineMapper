using System;
using System.ComponentModel.DataAnnotations;

namespace StateMachineMapper.Entities;

public class StateMachineTemplateConsumer
{
    public int Id { get; set; }
    [StringLength(500)]
    public string HandlerName { get; set; }
    public Guid TemplateId { get; set; }
    public StateMachineTemplate Template { get; set; }
}
