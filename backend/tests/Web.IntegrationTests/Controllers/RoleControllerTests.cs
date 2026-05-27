using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities.Roles;
using Web.Dtos.Roles;
using Web.IntegrationTests.Collections;
using Web.IntegrationTests.Environments;
using Web.IntegrationTests.Extensions;
using Web.IntegrationTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Controllers
{
    /// <summary>
    /// RoleControllerのインテグレーションテスト
    /// </summary>
    [Collection(CollectionDefinitionNames.Integration)]
    public class RoleControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly TestEnvironment _env;

        public RoleControllerTests(ITestOutputHelper outputHelper, TestEnvironment env)
        {
            _outputHelper = outputHelper;
            _env = env;
        }

        [Fact(DisplayName = "ロール一覧を取得できる")]
        public async Task GetRoles_ReturnsRoleList()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            using var db = _env.DbFixture.CreateDbContext(TestDbConnectionType.App);

            var roleEms = new[]
            {
                RoleEm.Create(RoleId.New(), RoleNameEnum.User, new RoleLabel("ユーザー"), RoleLevelEnum.User),
                RoleEm.Create(RoleId.New(), RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin),
                RoleEm.Create(RoleId.New(), RoleNameEnum.SystemAdmin, new RoleLabel("システム管理者"), RoleLevelEnum.SystemAdmin),
            };

            foreach (var role in roleEms)
            {
                db.Roles.Add(role);
            }
            await db.SaveChangesAsync();

            await _env.Factory.RunWithAuthenticationAsync([], async (client) =>
            {
                // 実行
                var response = await client.GetAsync("/api/roles");

                var responseJson = await response.Content.ReadFromJsonAsync<List<RoleDetailResponse>>(new JsonSerializerOptions()
                {
                    Converters = { new JsonStringEnumConverter() },
                });

                // 検証
                response.IsOk();
                Assert.NotNull(responseJson);
                Assert.Equal(roleEms.Length, responseJson.Count);
                for (var i = 0; i < roleEms.Length; i++)
                {
                    Assert.Equal(roleEms[i].Id.Value, responseJson[i].Id);
                    Assert.Equal(roleEms[i].Label.Value, responseJson[i].Label);
                    Assert.Equal(roleEms[i].Name.ToString(), responseJson[i].Name.ToString());
                    Assert.Equal((int)roleEms[i].Level, responseJson[i].Level);
                }
            });
        }
    }
}
