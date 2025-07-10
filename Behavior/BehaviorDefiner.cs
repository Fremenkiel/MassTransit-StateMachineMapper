using System;
using StateMachineMapper.Entities;

namespace StateMachineMapper.Behavior;

public static class BehaviorDefiner
{
    public static StateMachineTemplates GetBehavior()
    {
        return new StateMachineTemplates()
        {
            Id = Guid.NewGuid(),
            Entries = new() {
                // Behavior for the Initial state
                new() { InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "Then", ActionParameter = "SetSubscriptionInfo" },
                new() { InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "TransitionTo", ActionParameter = "Welcoming" },
                new() { InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "Publish", ActionParameter = "SendWelcomeEmail" },

                // Behavior for the Welcoming state
                new() { InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Then", ActionParameter = "MarkWelcomeEmailSent" },
                new() { InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "TransitionTo", ActionParameter = "FollowingUp" },
                new() { InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Publish", ActionParameter = "SendFollowUpEmail" },

                // Behavior for the FollowingUp state
                new() { InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Then", ActionParameter = "MarkFollowUpEmailSentAndComplete" },
                new() { InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "TransitionTo", ActionParameter = "Onboarding" },
                new() { InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Publish", ActionParameter = "PublishOnboardingCompleted" },
                new() { InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Finalize" },
            }
        };
    }
}