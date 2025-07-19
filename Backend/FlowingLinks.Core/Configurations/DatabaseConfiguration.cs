namespace FlowingLinks.Core.Configurations;

public class DatabaseConfiguration
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "Sqlite"; // Default to SQLite
    public string ConnectionString { get; set; } = string.Empty;
    
    // SQLite specific settings
    public string? SqliteDatabasePath { get; set; }
    
    // PostgreSQL specific settings
    public string? PostgresHost { get; set; }
    public int? PostgresPort { get; set; }
    public string? PostgresDatabase { get; set; }
    public string? PostgresUsername { get; set; }
    public string? PostgresPassword { get; set; }
} 