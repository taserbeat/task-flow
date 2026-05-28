using Web.Dtos.Version;
using Web.IntegrationTests.Collections;
using Web.IntegrationTests.Environments;
using Web.IntegrationTests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Controllers
{
    /// <summary>
    /// VersionControllerのインテグレーションテスト
    /// </summary>
    [Collection(CollectionDefinitionNames.Integration)]
    public class VersionControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly TestEnvironment _env;

        public VersionControllerTests(ITestOutputHelper outputHelper, TestEnvironment env)
        {
            _outputHelper = outputHelper;
            _env = env;
        }

        [Fact(DisplayName = "認証なしでバージョン情報を取得できる")]
        public async Task GetVersion_WithoutAuth_ReturnsOk()
        {
            await _env.Factory.RunWithoutAuthenticationAsync(async (client) =>
            {
                // 実行
                var response = await client.GetAsync("/api/version");
                var responsejson = await response.Content.ReadAsStringAsync();
                _outputHelper.WriteLine("Response JSON: {0}", responsejson);

                // 検証
                response.IsOk(new VersionInfo());
            });
        }
    }
}
