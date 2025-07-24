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
        Guid templateId2 = new Guid("82213420-f7f6-4259-ac9c-84bb46be35f9");

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

            new() { Id = 11, TemplateId = templateId2, InitialStateName = "Initially", TriggerEventName = "SignUpForNewsletter", ActionType = "Then", ActionParameter = "SetNewsletterInfo"},
            new() { Id = 12, TemplateId = templateId2, InitialStateName = "Initially", TriggerEventName = "SignUpForNewsletter", ActionType = "TransitionTo", ActionParameter = "Welcoming" },
            new() { Id = 13, TemplateId = templateId2,  InitialStateName = "Initially", TriggerEventName = "SignUpForNewsletter", ActionType = "Publish", ActionParameter = "SendWelcomeEmail" },

            new() { Id = 14, TemplateId = templateId2, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Then", ActionParameter = "MarkWelcomeEmailSent" },
            new() { Id = 15, TemplateId = templateId2, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "TransitionTo", ActionParameter = "FollowingUp" },
            new() { Id = 16, TemplateId = templateId2, InitialStateName = "Welcoming", TriggerEventName = "WelcomeEmailSent", ActionType = "Publish", ActionParameter = "SendFollowUpEmail" },

            new() { Id = 17, TemplateId = templateId2, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Then", ActionParameter = "MarkFollowUpEmailSentAndComplete" },
            new() { Id = 18, TemplateId = templateId2, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "TransitionTo", ActionParameter = "Onboarding" },
            new() { Id = 19, TemplateId = templateId2, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Publish", ActionParameter = "PublishOnboardingCompleted" },
            new() { Id = 20, TemplateId = templateId2, InitialStateName = "FollowingUp", TriggerEventName = "FollowUpEmailSent", ActionType = "Finalize" },
        };

        List<StateMachineTemplateConsumer> consumers = new ()
        {
            new() { Id = 1, TemplateId = templateId, HandlerName = "SendWelcomeEmailHandler" },
            new() { Id = 2, TemplateId = templateId, HandlerName = "SendFollowUpEmailHandler" },
            new() { Id = 3, TemplateId = templateId, HandlerName = "OnboardingCompletedHandler" },
            new() { Id = 4, TemplateId = templateId2, HandlerName = "SendWelcomeEmailHandler" },
            new() { Id = 5, TemplateId = templateId2, HandlerName = "SendFollowUpEmailHandler" },
            new() { Id = 6, TemplateId = templateId2, HandlerName = "OnboardingCompletedHandler" },
        };

        modelBuilder.Entity<StateMachineTemplate>().HasData(new List<StateMachineTemplate> {
            new()
            {
                Id = templateId
            },
            new()
            {
                Id = templateId2
            }});

        modelBuilder.Entity<StateMachineTemplateEntry>().HasData(entries);
        modelBuilder.Entity<StateMachineTemplateConsumer>().HasData(consumers);
    }
}
