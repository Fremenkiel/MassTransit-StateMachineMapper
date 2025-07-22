using System;
using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Events;
using StateMachineMapper.Interfaces;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class SendWelcomeEmailHandler : IConsumer<SendWelcomeEmail>
{
    private readonly IBus _bus;
    private readonly IEmailService _emailService;

    public SendWelcomeEmailHandler(IBus bus, IEmailService emailService)
    {
        _bus = bus;
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<SendWelcomeEmail> context)
    {
        await _emailService.SendWelcomeEmailAsync(context.Message.Email);

        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{context.Message.TemplateId}"));

        await endpoint.Send(new WelcomeEmailSent
        {
            SubscriptionId = context.Message.SubscriptionId,
            Email = context.Message.Email
        });
    }
}
