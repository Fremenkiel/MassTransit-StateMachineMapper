using System;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.Sagas.Data;

namespace StateMachineMapper.Sagas.Wrapper;

using MassTransit;
using Sagas;


public class ActivityWrapper
{
    public Action<BehaviorContext<OnboardingSagaData, ICorrelatableById>> Logic { get; }

    public ActivityWrapper(Action<BehaviorContext<OnboardingSagaData, ICorrelatableById>> logic)
    {
        Logic = logic;
    }
}
