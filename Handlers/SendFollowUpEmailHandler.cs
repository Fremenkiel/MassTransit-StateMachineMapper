using System;
using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Events;
using StateMachineMapper.Interfaces;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class SendFollowUpEmailHandler : IConsumer<SendFollowUpEmail>
{
    private readonly IBus _bus;
    private readonly IEmailService _emailService;

    public SendFollowUpEmailHandler(IBus bus, IEmailService emailService)
    {
        _bus = bus;
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<SendFollowUpEmail> context)
    {
        await _emailService.SendFollowUpEmailAsync(context.Message.Email);

        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{context.Message.TemplateId}"));

        await endpoint.Send(new FollowUpEmailSent
        {
            SubscriptionId = context.Message.SubscriptionId,
            Email = context.Message.Email
        });
    }
}
