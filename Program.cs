using System;
using System.Collections.Generic;
using StateMachineMapper.Commands;
using StateMachineMapper.Database;
using StateMachineMapper.Interfaces;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.Services;
using StateMachineMapper.StateMachine;
using MassTransit;
using MassTransit.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StateMachineMapper.Constants;
using StateMachineMapper.Manager;
using StateMachineMapper.StateMachine.Data;
using StateMachineMapper.StateMachine.Manager;
using StateMachineMapper.StateMachine.Manager.Interfaces;
using StateMachineMapper.StateMachine.TransientRegistrationConfigurator;

namespace StateMachineMapper;

public static class Program
{
    public static int Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Database")!, npgsqlDataSourceBuilder =>
        {
            npgsqlDataSourceBuilder.UseNetTopologySuite();
            npgsqlDataSourceBuilder.EnableDynamicJson();
        });

        var wDataSourceBuilder =
            new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Database")!);
        wDataSourceBuilder.UseNetTopologySuite();
        wDataSourceBuilder.EnableDynamicJson();
        var wDataSource = wDataSourceBuilder.Build();

        builder.Services.AddDbContext<DefaultDatabaseContext>(ctx =>
        {
            ctx.UseNpgsql(wDataSource, npgsqlDbContextOptionsBuilder =>
            {
                npgsqlDbContextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                npgsqlDbContextOptionsBuilder.UseNetTopologySuite();
            });
            ctx.ConfigureWarnings(wc => wc.Ignore(RelationalEventId.PendingModelChangesWarning).Ignore(RelationalEventId.BoolWithDefaultWarning).Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        });

        builder.Services.AddScoped<DbContext>(provider => provider.GetService<DefaultDatabaseContext>());

        builder.Services.AddTransientMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumers(typeof(Program).Assembly);

            busConfigurator.AddTransientSagaStateMachine<OnboardingStateMachine, OnboardingStateMachineData>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<DefaultDatabaseContext>();

                    r.UsePostgres();
                }).ExcludeFromConfigureEndpoints();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["AppSettings:RabbitMq:Host"], builder.Configuration["AppSettings:RabbitMq:VHost"], h =>
                {
                    h.Username(builder.Configuration["AppSettings:RabbitMq:Username"]!);
                    h.Password(builder.Configuration["AppSettings:RabbitMq:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(ConfigureResource)
                .WithTracing(b => b
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(builder.Environment.ApplicationName)
                        .AddTelemetrySdk()
                        .AddAttributes(
                            new KeyValuePair<string, object>[]
                            {
                                new("deployment.environment", builder.Environment.EnvironmentName),
                            })
                        .AddEnvironmentVariableDetector())
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.RecordException = true;
                    })
                    .AddHttpClientInstrumentation(o =>
                    {
                        o.RecordException = true;
                    })
                    .AddConsoleExporter()
                );
        });

        builder.Services.AddSingleton<EndpointManager>();
        builder.Services.AddSingleton<RabbitMqManager>();
        builder.Services.AddSingleton<IEmailService, EmailService>();

        builder.Services.AddTransient<IDynamicStateMachineManager, DynamicStateMachineManager>();

        builder.Services.AddMvc();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            if (endpoints != null)
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            }
        });

        app.UseSwagger();
        app.UseSwaggerUI();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultDatabaseContext>();
        context.Database.Migrate();

        app.UseHttpsRedirection();

        app.Run();

        return 0;
    }

    private static void ConfigureResource(this ResourceBuilder r)
    {
        r.AddService("State Machine Mapper",
            serviceVersion: "0.0.1",
            serviceInstanceId: Environment.MachineName);
    }
}
