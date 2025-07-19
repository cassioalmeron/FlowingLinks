using System.Reflection;
using FlowingLinks.Core.Models;
using FlowingLinks.Core.Utils;
using FlowingLinks.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Core;

public class FlowingLinksDbContext : DbContext
{
    private readonly DatabaseConfiguration _databaseConfig;

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<LinkLabel> LinkLabels { get; set; }

    public FlowingLinksDbContext(DatabaseConfiguration? databaseConfig = null)
        => _databaseConfig = databaseConfig ?? new DatabaseConfiguration();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            ConfigureDatabase(optionsBuilder);
    }

    private void ConfigureDatabase(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_databaseConfig.Provider.ToLowerInvariant())
        {
            case "postgresql":
            case "postgres":
                ConfigurePostgreSQL(optionsBuilder);
                break;
            case "sqlite":
            default:
                ConfigureSQLite(optionsBuilder);
                break;
        }
    }

    private void ConfigureSQLite(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        path = Path.Combine(path, "FlowingLinks", "Database");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var dbPath = Path.Combine(path, _databaseConfig.SqliteDatabasePath ?? "FlowingLinks.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    private void ConfigurePostgreSQL(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = BuildPostgreSQLConnectionString();
        optionsBuilder.UseNpgsql(connectionString);
    }

    private string BuildPostgreSQLConnectionString()
    {
        if (!string.IsNullOrEmpty(_databaseConfig.ConnectionString))
            return _databaseConfig.ConnectionString;

        var host = _databaseConfig.PostgresHost ?? "localhost";
        var port = _databaseConfig.PostgresPort ?? 5432;
        var database = _databaseConfig.PostgresDatabase ?? "flowinglinks";
        var username = _databaseConfig.PostgresUsername ?? "postgres";
        var password = _databaseConfig.PostgresPassword ?? "";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Ensures that the database exists and the admin user is created (synchronous version)
    /// </summary>
    public void EnsureDatabaseAndAdminUser()
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
                Password = HashUtils.GenerateMd5Hash("admin")
            };

            Users.Add(adminUser);
            SaveChanges();
        }
    }
}