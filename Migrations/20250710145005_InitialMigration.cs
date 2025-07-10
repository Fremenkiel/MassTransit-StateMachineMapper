using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StateMachineMapper.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "OnboardingStateMachineData",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WelcomeEmailSent = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpEmailSent = table.Column<bool>(type: "boolean", nullable: false),
                    OnboardingCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingStateMachineData", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineTemplateConsumers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HandlerName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineTemplateConsumers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateMachineTemplateConsumers_StateMachineTemplates_Templat~",
                        column: x => x.TemplateId,
                        principalTable: "StateMachineTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StateMachineTemplateEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InitialStateName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TriggerEventName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ActionType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ActionParameter = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMachineTemplateEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateMachineTemplateEntries_StateMachineTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "StateMachineTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StateMachineTemplates",
                column: "Id",
                value: new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"));

            migrationBuilder.InsertData(
                table: "StateMachineTemplateConsumers",
                columns: new[] { "Id", "HandlerName", "TemplateId" },
                values: new object[,]
                {
                    { 1, "OnboardingHandler", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95") },
                    { 2, "SendWelcomeEmailHandler", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95") },
                    { 3, "SendFollowUpEmailHandler", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95") },
                    { 4, "OnboardingCompletedHandler", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95") }
                });

            migrationBuilder.InsertData(
                table: "StateMachineTemplateEntries",
                columns: new[] { "Id", "ActionParameter", "ActionType", "InitialStateName", "TemplateId", "TriggerEventName" },
                values: new object[,]
                {
                    { 1, "SetSubscriptionInfo", "Then", "Initially", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "SubscriberCreated" },
                    { 2, "Welcoming", "TransitionTo", "Initially", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "SubscriberCreated" },
                    { 3, "SendWelcomeEmail", "Publish", "Initially", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "SubscriberCreated" },
                    { 4, "MarkWelcomeEmailSent", "Then", "Welcoming", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "WelcomeEmailSent" },
                    { 5, "FollowingUp", "TransitionTo", "Welcoming", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "WelcomeEmailSent" },
                    { 6, "SendFollowUpEmail", "Publish", "Welcoming", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "WelcomeEmailSent" },
                    { 7, "MarkFollowUpEmailSentAndComplete", "Then", "FollowingUp", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "FollowUpEmailSent" },
                    { 8, "Onboarding", "TransitionTo", "FollowingUp", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "FollowUpEmailSent" },
                    { 9, "PublishOnboardingCompleted", "Publish", "FollowingUp", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "FollowUpEmailSent" },
                    { 10, null, "Finalize", "FollowingUp", new Guid("e54baee0-f1a6-4e50-9e92-4f65bed95c95"), "FollowUpEmailSent" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineTemplateConsumers_TemplateId",
                table: "StateMachineTemplateConsumers",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StateMachineTemplateEntries_TemplateId",
                table: "StateMachineTemplateEntries",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnboardingStateMachineData");

            migrationBuilder.DropTable(
                name: "StateMachineTemplateConsumers");

            migrationBuilder.DropTable(
                name: "StateMachineTemplateEntries");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "StateMachineTemplates");
        }
    }
}
