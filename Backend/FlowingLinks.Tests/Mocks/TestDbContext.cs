using FlowingLinks.Core;
using FlowingLinks.Core.Models;
using FlowingLinks.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Tests.Mocks;

internal class TestDbContext : FlowingLinksDbContext
{
    private static int _count;

    public TestDbContext() : base(new DatabaseConfiguration())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Override the base configuration to use in-memory database for tests
        optionsBuilder.UseInMemoryDatabase($"Virtual-Database-Name-{_count++}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    /// <summary>
    /// Initialize the test database with admin user
    /// </summary>
    public void InitializeTestDatabase()
    {
        // Ensure database is created
        Database.EnsureCreated();

        // Check if admin user exists
        var adminUser = Users.Find(1);

        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = 1,
                Name = "Administrator",
                Username = "admin",
                Password = "admin" // Use plain text for tests
            };

            Users.Add(adminUser);
            SaveChanges();
        }
    }
}