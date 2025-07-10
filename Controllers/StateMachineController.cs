using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.StateMachine.Manager.Interfaces;

namespace StateMachineMapper.Controllers;

[Route("api/state-machine")]
[ApiController]
[AllowAnonymous]
public class StateMachineController : ControllerBase
{
    private readonly IDynamicStateMachineManager _dynamicStateMachineManager;

    public StateMachineController(IDynamicStateMachineManager dynamicStateMachineManager)
    {
        _dynamicStateMachineManager = dynamicStateMachineManager;
    }

    [HttpPost]
    public async Task<IActionResult> Start([FromBody] Guid queryName)
    {
        await _dynamicStateMachineManager.ConnectStateMachine<OnboardingStateMachineData>(queryName);
        return Ok();
    }

    [HttpDelete("{queryName:guid}")]
    public async Task<IActionResult> Stop(Guid queryName)
    {
        await _dynamicStateMachineManager.DisconnectStateMachine(queryName);
        return Ok();
    }
}
