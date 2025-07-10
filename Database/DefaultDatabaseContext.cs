using StateMachineMapper.Entities;
using StateMachineMapper.StateMachine.Data;
using Microsoft.EntityFrameworkCore;

namespace StateMachineMapper.Database;

public partial class DefaultDatabaseContext : DbContext
{
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OnboardingStateMachineData>().HasKey(s => s.CorrelationId);

        modelBuilder.Entity<StateMachineTemplateEntry>()
            .HasOne(x => x.Template)
            .WithMany(x => x.Entries)
            .HasForeignKey(s => s.TemplateId);

        modelBuilder.Entity<StateMachineTemplateConsumer>()
            .HasOne(x => x.Template)
            .WithMany(x => x.Consumers)
            .HasForeignKey(s => s.TemplateId);

        SeedData(modelBuilder);
    }

    public DbSet<Subscriber> Subscribers { get; set; }

    public DbSet<OnboardingStateMachineData> OnboardingStateMachineData { get; set; }

    public DbSet<StateMachineTemplateConsumer> StateMachineTemplateConsumers { get; set; }
    public DbSet<StateMachineTemplateEntry> StateMachineTemplateEntries { get; set; }
    public DbSet<StateMachineTemplate> StateMachineTemplates { get; set; }
}
