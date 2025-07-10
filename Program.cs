using System;
using StateMachineMapper.Commands;
using StateMachineMapper.Database;
using StateMachineMapper.Interfaces;
using StateMachineMapper.Sagas.Data;
using StateMachineMapper.Services;
using StateMachineMapper.StateMachine;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace StateMachineMapper;

public static class Program
{
    public static int Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Database")!, builder =>
        {
            builder.UseNetTopologySuite();
            builder.EnableDynamicJson();
        });
        
        var wDataSourceBuilder =
            new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Database")!);
        wDataSourceBuilder.UseNetTopologySuite();
        wDataSourceBuilder.EnableDynamicJson();
        var wDataSource = wDataSourceBuilder.Build();

        builder.Services.AddDbContext<DefaultDatabaseContext>((ctx) =>
        {
            ctx.UseNpgsql(wDataSource, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                builder.UseNetTopologySuite();
            });
            ctx.ConfigureWarnings(wc => wc.Ignore(RelationalEventId.PendingModelChangesWarning).Ignore(RelationalEventId.BoolWithDefaultWarning).Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        });

        builder.Services.AddScoped<DbContext>(provider => provider.GetService<DefaultDatabaseContext>());

        builder.Services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumers(typeof(Program).Assembly);

            busConfigurator.AddSagaStateMachine<OnboardingSaga, OnboardingSagaData>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<DefaultDatabaseContext>();

                    r.UsePostgres();
                });
            
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                });
                
                cfg.ConfigureEndpoints(context);
            });

        });

        builder.Services.AddTransient<IEmailService, EmailService>();
        
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
}
