using System.Linq;
using StateMachineMapper.Entities;

namespace StateMachineMapper.StateMachine.Builder.Interfaces;

public interface IEventBehaviorBuilder
{
    void Build(OnboardingStateMachine stateMachine, IGrouping<string,StateMachineTemplateEntry> behaviorGroup);
}
