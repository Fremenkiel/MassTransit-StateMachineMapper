using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StateMachineMapper.Entities;

namespace StateMachineMapper.Database;

public partial class DefaultDatabaseContext
{

    private void SeedData(ModelBuilder modelBuilder)
    {
        Guid templateId = new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95");

        List<StateMachineTemplateEntry> entries = new ()
        {
            new() { Id = 1, TemplateId = templateId, InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "Then", ActionParameter = "SetSubscriptionInfo"},
            new() { Id = 2, TemplateId = templateId, InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "TransitionTo", ActionParameter = "Welcoming" },
            new() { Id = 3, TemplateId = templateId,  InitialStateName = "Initially", TriggerEventName = "SubscriberCreated", ActionType = "Publish", ActionParameter = "SendWelcomeEmail" },

            new() { Id = 4, TemplateId = templateId, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Then", ActionParameter = "MarkWelcomeEmailSent" },
            new() { Id = 5, TemplateId = templateId, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "TransitionTo", ActionParameter = "FollowingUp" },
            new() { Id = 6, TemplateId = templateId, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Publish", ActionParameter = "SendFollowUpEmail" },

            new() { Id = 7, TemplateId = templateId, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Then", ActionParameter = "MarkFollowUpEmailSentAndComplete" },
            new() { Id = 8, TemplateId = templateId, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "TransitionTo", ActionParameter = "Onboarding" },
            new() { Id = 9, TemplateId = templateId, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Publish", ActionParameter = "PublishOnboardingCompleted" },
            new() { Id = 10, TemplateId = templateId, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Finalize" },
        };

        List<StateMachineTemplateConsumer> consumers = new ()
        {
            new() { Id = 1, TemplateId = templateId, HandlerName = "OnboardingHandler"},
            new() { Id = 2, TemplateId = templateId, HandlerName = "SendWelcomeEmailHandler" },
            new() { Id = 3, TemplateId = templateId, HandlerName = "SendFollowUpEmailHandler" },
            new() { Id = 4, TemplateId = templateId, HandlerName = "OnboardingCompletedHandler" },
        };

        modelBuilder.Entity<StateMachineTemplate>().HasData(
            new StateMachineTemplate
            {
                Id = templateId,
            });

        modelBuilder.Entity<StateMachineTemplateEntry>().HasData(entries);
        modelBuilder.Entity<StateMachineTemplateConsumer>().HasData(consumers);
    }
}
