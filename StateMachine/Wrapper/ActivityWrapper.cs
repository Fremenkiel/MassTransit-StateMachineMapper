using System;
using StateMachineMapper.Events.Interfaces;
using StateMachineMapper.StateMachine.Data;
using MassTransit;

namespace StateMachineMapper.StateMachine.Wrapper;

public class ActivityWrapper
{
    public Action<BehaviorContext<OnboardingStateMachineData, ICorrelatableById>> Logic { get; }

    public ActivityWrapper(Action<BehaviorContext<OnboardingStateMachineData, ICorrelatableById>> logic)
    {
        Logic = logic;
    }
}
