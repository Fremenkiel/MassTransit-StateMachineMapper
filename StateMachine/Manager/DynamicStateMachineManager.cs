using System;
using System.Linq;
using System.Threading.Tasks;
using StateMachineMapper.Database;
using StateMachineMapper.Handlers;
using StateMachineMapper.Manager;
using StateMachineMapper.StateMachine.Manager.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace StateMachineMapper.StateMachine.Manager;

public class DynamicStateMachineManager : IDynamicStateMachineManager
{
    private readonly IReceiveEndpointConnector _receiveEndpointConnector;
    private readonly DefaultDatabaseContext _dbContext;
    private readonly EndpointManager _endpointManager;
    private readonly RabbitMqManager _rabbitMqManager;

    public DynamicStateMachineManager(IReceiveEndpointConnector receiveEndpointConnector,
        DefaultDatabaseContext dbContext, EndpointManager endpointManager,
        RabbitMqManager rabbitMqManager)
    {
        _receiveEndpointConnector = receiveEndpointConnector;
        _dbContext = dbContext;
        _endpointManager = endpointManager;
        _rabbitMqManager = rabbitMqManager;
    }

    public async Task ConnectStateMachine<TState>(Guid queueName)
        where TState : class, SagaStateMachineInstance, new()
    {
        _endpointManager.AssignEndpointName(queueName);

        var template = await _dbContext.StateMachineTemplates
            .Include(x => x.Entries)
            .Include(x => x.Consumers)
            .Where(x => x.Id == queueName).FirstOrDefaultAsync();

        if (template is null)
            return;

        _endpointManager.AssignEndpointStateMachineTemplate(template);

        HostReceiveEndpointHandle handle = null;
        handle = _receiveEndpointConnector.ConnectReceiveEndpoint(queueName.ToString(), (context, cfg) =>
        {
            cfg.PrefetchCount = 4;
            foreach (var customer in template.Consumers)
            {
                cfg.ConfigureConsumer(context, typeof(HandlersClassesAssemblyHelper).Assembly.GetTypes().First(x => x.Name == customer.HandlerName && x.Namespace == HandlersClassesAssemblyHelper.Namespace));
            }

            cfg.StateMachineSaga<TState>(context, stateMachineConfigurator =>
            {
                stateMachineConfigurator.UsePartitioner(8, m => m.Saga.CorrelationId);
            });

        });

        _endpointManager.AddEndpoint(handle);

        await handle.Ready;
    }

    public async Task DisconnectStateMachine(Guid queueName)
    {
        await _endpointManager.RemoveEndpointAsync(queueName);
        await _rabbitMqManager.DeleteEndpointTopology(queueName.ToString());
    }
}
