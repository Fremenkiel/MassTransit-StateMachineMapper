using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StateMachineMapper.Commands;

namespace StateMachineMapper.Controllers;

[Route("api/onboarding")]
[ApiController]
[AllowAnonymous]
public class OnboardingController : ControllerBase
{
    private readonly IBus _bus;
    
    public OnboardingController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string email)
    {
        await _bus.Publish(new Onboarding(email));
        return Accepted();
    }
    
}
