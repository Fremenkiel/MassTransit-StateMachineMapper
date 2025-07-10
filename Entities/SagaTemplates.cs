using System;
using System.Collections.Generic;

namespace StateMachineMapper.Entities;

public class StateMachineTemplates
{
    public Guid Id { get; set; }
    public List<SagStateMachineTemplateEntry> Entries { get; set; }
}
