using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Events;
using StateMachineMapper.Interfaces;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class SendWelcomeEmailHandler : IConsumer<SendWelcomeEmail>
{
    private readonly IEmailService _emailService;

    public SendWelcomeEmailHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<SendWelcomeEmail> context)
    {
        await _emailService.SendWelcomeEmailAsync(context.Message.Email);

        await context.Publish(new WelcomeEmailSent
        {
            SubscriptionId = context.Message.SubscriptionId,
            Email = context.Message.Email
        });
    }
}
