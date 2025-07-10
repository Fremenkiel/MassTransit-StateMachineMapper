using System;
using System.Collections.Generic;

namespace StateMachineMapper.Entities;

public class StateMachineTemplate
{
    public Guid Id { get; set; }
    public List<StateMachineTemplateEntry> Entries { get; set; }
    public List<StateMachineTemplateConsumer> Consumers { get; set; }
}
