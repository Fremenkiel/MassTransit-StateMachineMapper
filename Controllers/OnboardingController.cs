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

    [HttpPost("{queryName:guid}")]
    public async Task<IActionResult> Post(Guid queryName, [FromBody] string email)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queryName}"));
        await endpoint.Send(new Onboarding(email));

        return Accepted();
    }

}
