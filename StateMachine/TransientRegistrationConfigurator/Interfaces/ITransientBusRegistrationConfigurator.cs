using MassTransit;

namespace StateMachineMapper.StateMachine.TransientRegistrationConfigurator.Interfaces;

public interface ITransientBusRegistrationConfigurator :
    IBusRegistrationConfigurator,
    ITransientRegistrationConfigurator
{

}
