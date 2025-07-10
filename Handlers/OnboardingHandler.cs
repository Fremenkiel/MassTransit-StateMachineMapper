using System;
using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Database;
using StateMachineMapper.Entities;
using StateMachineMapper.Events;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class OnboardingHandler : IConsumer<Onboarding>
{
    private readonly DefaultDatabaseContext _dbContext;

    public OnboardingHandler(DefaultDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<Onboarding> context)
    {
        var subscription = _dbContext.Subscribers.Add(new Subscriber
        {
            Id = Guid.NewGuid(),
            Email = context.Message.Email,
            CreatedAt = DateTimeOffset.Now
        });

        await _dbContext.SaveChangesAsync();


        await context.Publish(new SubscriberCreated
        {
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            SubscriptionId = subscription.Entity.Id,
            Email = subscription.Entity.Email
        });
    }
}
