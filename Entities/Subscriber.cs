using System;

namespace StateMachineMapper.Entities;

public class Subscriber
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}