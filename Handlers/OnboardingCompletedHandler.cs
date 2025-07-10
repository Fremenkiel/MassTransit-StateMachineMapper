using System.Threading.Tasks;
using StateMachineMapper.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace StateMachineMapper.Handlers;

public class OnboardingCompletedHandler : IConsumer<OnboardingCompleted>
{
    private readonly ILogger<OnboardingCompleted> _logger;
    
    public OnboardingCompletedHandler(ILogger<OnboardingCompleted> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<OnboardingCompleted> context)
    {
        _logger.LogInformation("Received OnboardingCompleted event");
        return Task.CompletedTask;
    }
}