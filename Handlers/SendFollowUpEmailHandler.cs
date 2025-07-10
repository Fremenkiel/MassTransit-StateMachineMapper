using System.Threading.Tasks;
using StateMachineMapper.Commands;
using StateMachineMapper.Events;
using StateMachineMapper.Interfaces;
using MassTransit;

namespace StateMachineMapper.Handlers;

public class SendFollowUpEmailHandler : IConsumer<SendFollowUpEmail>
{
    private readonly IEmailService _emailService;

    public SendFollowUpEmailHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<SendFollowUpEmail> context)
    {
        await _emailService.SendFollowUpEmailAsync(context.Message.Email);

        await context.Publish(new FollowUpEmailSent
        {
            SubscriptionId = context.Message.SubscriptionId,
            Email = context.Message.Email
        });
    }
}
