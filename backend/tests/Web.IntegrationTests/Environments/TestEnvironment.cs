using Web.IntegrationTests.Factories;
using Web.IntegrationTests.Fixtures;
using Xunit;

namespace Web.IntegrationTests.Environments
{
    /// <summary>
    /// テスト環境全体のセットアップとクリーンアップを行うクラス
    /// </summary>
    public class TestEnvironment : IAsyncLifetime
    {
        public TestDataBaseFixture DbFixture { get; }
        public CustomWebApplicationFactory Factory { get; }

        public TestEnvironment()
        {
            DbFixture = new();
            Factory = new(DbFixture);
        }

        public async Task InitializeAsync()
        {
            await DbFixture.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            await DbFixture.DisposeAsync();
        }
    }
}