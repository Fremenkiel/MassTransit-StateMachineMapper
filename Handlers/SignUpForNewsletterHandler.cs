using System;
using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Database;
using StateMachineMapper.Entities;
using StateMachineMapper.Events;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class SignUpForNewsletterHandler : IConsumer<SignUp>
{
    private readonly IBus _bus;
    private readonly DefaultDatabaseContext _dbContext;

    public SignUpForNewsletterHandler(IBus bus, DefaultDatabaseContext dbContext)
    {
        _bus = bus;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<SignUp> context)
    {
        var subscription = _dbContext.Subscribers.Add(new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = context.Message.Email,
            CreatedAt = DateTimeOffset.Now
        });

        await _dbContext.SaveChangesAsync();

        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{context.Message.TemplateId}"));

        await endpoint.Send(new SignUpForNewsletter
        {
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            SubscriptionId = subscription.Entity.Id,
            TemplateId = context.Message.TemplateId,
            Email = subscription.Entity.Email
        });
    }
}
