using Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.IntegrationTests.Contexts;
using Web.IntegrationTests.Fixtures;
using Web.IntegrationTests.Handlers;

namespace Web.IntegrationTests.Factories
{
    /// <summary>
    /// テスト用のWebApplicationFactory
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly TestDataBaseFixture _dbFixture;
        public TestAuthContext AuthContext { get; set; }

        public CustomWebApplicationFactory(TestDataBaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            AuthContext = new();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // テスト用のappsettings.jsonを読み込む
                config.AddJsonFile("appsettings.Test.json");
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureTestServices(services =>
            {
                // Webサービスのみをテストするため、その他のHostedServiceは削除
                ServiceCollectionDescriptorExtensions.RemoveAll(services, typeof(IHostedService));

                services.AddSingleton(AuthContext);

                // 本番の認証を削除し、テスト用のモック認証を追加
                services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                services.RemoveAll<IPostConfigureOptions<AuthenticationOptions>>();
                services.RemoveAll<IAuthenticationSchemeProvider>();

                services.PostConfigureAll<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme = AuthMockHandler.DefaultScheme;
                    options.DefaultAuthenticateScheme = AuthMockHandler.DefaultScheme;
                    options.DefaultChallengeScheme = AuthMockHandler.DefaultScheme;
                });

                services
                    .AddAuthentication(AuthMockHandler.DefaultScheme)
                    .AddScheme<AuthenticationSchemeOptions, AuthMockHandler>(AuthMockHandler.DefaultScheme, _ =>
                    {

                    });

                // テスト用のDB接続に差し替え
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                var provider = services.BuildServiceProvider();
                var config = provider.GetRequiredService<IConfiguration>();

                var connectionString = _dbFixture.GetConnectionString(TestDbConnectionType.App);
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });
            });
        }
    }
}
