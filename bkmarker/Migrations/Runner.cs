using FluentMigrator.Runner;

namespace bkmarker.Migrations;

class Runner
{
    public static void MigrateUp(ConnectionString connectionString)
    {
        using (var serviceProvider = CreateServices(connectionString))
        using (var scope = serviceProvider.CreateScope())
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            UpdateDatabase(scope.ServiceProvider);
        }
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static ServiceProvider CreateServices(ConnectionString connectionString)
    {
        return new ServiceCollection()
          // Add common FluentMigrator services
          .AddFluentMigratorCore()
          .ConfigureRunner(rb => rb
              // Add SQLite support to FluentMigrator
              .AddSQLite()
              // Set the connection string
              .WithGlobalConnectionString(connectionString.Value)
              // Define the assembly containing the migrations
              .ScanIn(typeof(Runner).Assembly).For.Migrations())
          // Enable logging to console in the FluentMigrator way
          .AddLogging(lb => lb.AddFluentMigratorConsole())
          // Build the service provider
          .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }
}
