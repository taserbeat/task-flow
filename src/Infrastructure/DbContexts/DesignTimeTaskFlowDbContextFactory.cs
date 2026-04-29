using Infrastructure.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DbContexts
{
    /// <summary>
    /// EF Coreのマイグレーション時に呼び出される<see cref="TaskFlowDbContext" />のファクトリー
    /// </summary>
    public class DesignTimeTaskFlowDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
    {
        public TaskFlowDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(ConnectionStringNames.MigrateConnection);
            var optionBuilder = new DbContextOptionsBuilder<TaskFlowDbContext>();
            optionBuilder.UseNpgsql(connectionString, optionBuilder =>
            {
                optionBuilder.MigrationsHistoryTable(TaskFlowDbContext.MigrationsHistoryTableName, TaskFlowDbContext.SchemaName);
            });

            return new TaskFlowDbContext(optionBuilder.Options);
        }
    }
}