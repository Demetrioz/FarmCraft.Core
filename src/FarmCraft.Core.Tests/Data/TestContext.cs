using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmCraft.Core.Tests.Data
{
    public class TestContext : DbContext, IFarmCraftContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {
        }

        public DbSet<FarmCraftLog> Logs { get; set; }
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Test");

            modelBuilder.Entity<FarmCraftLog>()
                .ToContainer("Logs")
                .HasNoDiscriminator()
                .HasPartitionKey(l => l.LogId);

            modelBuilder.Entity<TestEntity>()
                .ToContainer("Tests")
                .HasNoDiscriminator()
                .HasPartitionKey(t => t.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
