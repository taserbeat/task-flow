using DotNet.Testcontainers.Images;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace Web.IntegrationTests.Fixtures
{
    /// <summary>
    /// テスト用のDB Fixture
    /// </summary>
    public class TestDataBaseFixture : IAsyncLifetime
    {
        private readonly IConfiguration _configuration;
        private readonly PostgreSqlContainer _container;

        public TestDataBaseFixture()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: false)
                .Build();

            var imageName = _configuration["TestDb:Image"];
            var database = _configuration["TestDb:Database"];
            var user = _configuration["TestDb:User"];
            var password = _configuration["TestDb:Password"];

            _container = new PostgreSqlBuilder(new DockerImage(imageName))
                .WithImagePullPolicy(PullPolicy.Missing)
                .WithDatabase(database)
                .WithUsername(user)
                .WithPassword(password)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            using var dbContext = CreateDbContext(TestDbConnectionType.Migrate);

            await dbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        public AppDbContext CreateDbContext(TestDbConnectionType connectionType)
        {
            var connectionString = GetConnectionString(connectionType);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);
        }

        public async Task ResetDatabaseAsync()
        {
            using var dbContext = CreateDbContext(TestDbConnectionType.Migrate);

            var connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["tf"],
            });

            await respawner.ResetAsync(connection);
        }

        public string GetConnectionString(TestDbConnectionType connectionType)
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(_container.GetConnectionString());

            switch (connectionType)
            {
                case TestDbConnectionType.App:
                    builder.Username = _configuration["DefaultConnection:User"] ?? builder.Username;
                    builder.Password = _configuration["DefaultConnection:Password"] ?? builder.Password;
                    break;
                case TestDbConnectionType.Migrate:
                    builder.Username = _configuration["MigrateConnection:User"] ?? builder.Username;
                    builder.Password = _configuration["MigrateConnection:Password"] ?? builder.Password;
                    break;
            }

            return builder.ToString();
        }
    }

    public enum TestDbConnectionType
    {
        App,
        Migrate
    }
}