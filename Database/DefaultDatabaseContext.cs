using StateMachineMapper.Entities;
using StateMachineMapper.Sagas.Data;
using Microsoft.EntityFrameworkCore;

namespace StateMachineMapper.Database;

public partial class DefaultDatabaseContext : DbContext
{
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options)
    {
    }
    
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OnboardingSagaData>().HasKey(s => s.CorrelationId);
    }
    
    public DbSet<Subscriber> Subscribers { get; set; }
    public DbSet<OnboardingSagaData> OnboardingSagaData { get; set; }
    
}