using FlowingLinks.Core;
using FlowingLinks.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Console;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--fix-migrations")
        {
            FixMigrations();
            return;
        }

        System.Console.WriteLine("Hello, World!");
    }

    static void FixMigrations()
    {
        var config = new DatabaseConfiguration { Provider = "Sqlite" };
        using var context = new FlowingLinksDbContext(config);

        System.Console.WriteLine("Checking database state...");

        // Check if __EFMigrationsHistory table exists
        var tableExists = context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE name = '__EFMigrationsHistory' AND type = 'table'").ToList().FirstOrDefault();

        System.Console.WriteLine($"__EFMigrationsHistory table exists: {tableExists > 0}");

        if (tableExists > 0)
        {
            // Check what migrations are recorded
            var migrations = context.Database.SqlQueryRaw<string>(
                "SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId").ToList();
            
            System.Console.WriteLine("Current migrations in history:");
            foreach (var migration in migrations)
            {
                System.Console.WriteLine($"  - {migration}");
            }
            
            if (!migrations.Contains("20250718235110_InicialMigration"))
            {
                System.Console.WriteLine("Initial migration not found in history. Adding it...");
                
                // Insert the initial migration record
                context.Database.ExecuteSqlRaw(
                    "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20250718235110_InicialMigration', '7.0.11')");
                
                System.Console.WriteLine("Initial migration record added successfully!");
            }
            else
            {
                System.Console.WriteLine("Initial migration already exists in history.");
            }
        }
        else
        {
            System.Console.WriteLine("__EFMigrationsHistory table does not exist. Creating it...");
            
            // Create the __EFMigrationsHistory table
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE __EFMigrationsHistory (
                    MigrationId TEXT PRIMARY KEY,
                    ProductVersion TEXT NOT NULL
                )");
            
            // Insert the initial migration record
            context.Database.ExecuteSqlRaw(
                "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20250718235110_InicialMigration', '7.0.11')");
            
            System.Console.WriteLine("__EFMigrationsHistory table created and initial migration record added!");
        }

        System.Console.WriteLine("Done!");
    }
}
