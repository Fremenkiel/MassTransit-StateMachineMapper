using System.Threading.Tasks;

namespace StateMachineMapper.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email);
    
    Task SendFollowUpEmailAsync(string email);
}