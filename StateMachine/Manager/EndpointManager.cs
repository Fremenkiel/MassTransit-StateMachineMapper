using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StateMachineMapper.Entities;
using MassTransit;

namespace StateMachineMapper.StateMachine.Manager;

public class EndpointManager
{
    private readonly Dictionary<Guid, HostReceiveEndpointHandle> _dynamicEndpointHandleCollection = new();
    private Guid? _endpointName;
    private StateMachineTemplate _stateMachineTemplate;

    public void AssignEndpointName(Guid endpointName)
    {
        _endpointName = endpointName;
    }

    public void AssignEndpointStateMachineTemplate(StateMachineTemplate stateMachineTemplate)
    {
        _stateMachineTemplate = stateMachineTemplate;
    }

    public StateMachineTemplate GetStateMachineTemplate() => _stateMachineTemplate;

    public void AddEndpoint(HostReceiveEndpointHandle handle)
    {
        if (!_endpointName.HasValue)
            return;

        _dynamicEndpointHandleCollection[_endpointName.Value] = handle;

        _endpointName = null;
        _stateMachineTemplate = null;
    }

    public async Task RemoveEndpointAsync(Guid endpointName)
    {
        AssignEndpointName(endpointName);
        await RemoveEndpointAsync();
    }

    private async Task RemoveEndpointAsync()
    {
        if (!_endpointName.HasValue)
            return;

        await _dynamicEndpointHandleCollection[_endpointName.Value].StopAsync();
        _dynamicEndpointHandleCollection.Remove(_endpointName.Value);
    }
    public bool ContainsEndpoint(Guid endpointName) => _dynamicEndpointHandleCollection.ContainsKey(endpointName);

}
