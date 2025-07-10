using System.Threading.Tasks;
using StateMachineMapper.Interfaces;

namespace StateMachineMapper.Services;

public class EmailService : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email)
    {
        await Task.Delay(1000);
    }
    public async Task SendFollowUpEmailAsync(string email)
    {
        await Task.Delay(1000);
    }
}