using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StateMachineMapper.Commands;

namespace StateMachineMapper.Controllers;

[Route("api/newsletter")]
[ApiController]
[AllowAnonymous]
public class SignUpForNewsletterController : ControllerBase
{
    private readonly IBus _bus;

    public SignUpForNewsletterController(IBus bus)
    {
        _bus = bus;
    }

    [HttpPost("{queryName:guid}")]
    public async Task<IActionResult> Post(Guid queryName, [FromBody] string email)
    {
        await _bus.Publish(new SignUp(queryName, email));

        return Accepted();
    }

}
