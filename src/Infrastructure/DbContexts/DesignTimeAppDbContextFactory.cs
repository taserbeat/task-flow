using Infrastructure.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DbContexts
{
    /// <summary>
    /// EF Coreのマイグレーション時に呼び出される<see cref="AppDbContext" />のファクトリー
    /// </summary>
    public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(ConnectionStringNames.MigrateConnection);
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseNpgsql(connectionString, optionBuilder =>
            {
                optionBuilder.MigrationsHistoryTable(AppDbContext.MigrationsHistoryTableName, AppDbContext.SchemaName);
            });

            return new AppDbContext(optionBuilder.Options);
        }
    }
}