using System.Net.Http.Json;
using Domain.Entities.Tenants;
using Microsoft.EntityFrameworkCore;
using Web.Dtos.Tenants.CreateTenant;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Tenants.UpdateTenant;
using Web.IntegrationTests.Collections;
using Web.IntegrationTests.Environments;
using Web.IntegrationTests.Extensions;
using Web.IntegrationTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Controllers
{
    /// <summary>
    /// TenantControllerのインテグレーションテスト
    /// </summary>
    [Collection(CollectionDefinitionNames.Integration)]
    public class TenantControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly TestEnvironment _env;

        public TenantControllerTests(ITestOutputHelper outputHelper, TestEnvironment env)
        {
            _outputHelper = outputHelper;
            _env = env;
        }

        #region テナントの作成

        [Fact(DisplayName = "システム管理者権限でテナントを作成できる(200)")]
        public async Task CreateTenant_ReturnsCreatedTenant()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();
            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "admin@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "田中",
                    FirstName = "太郎",
                    RoleId = _env.AdminRole.Id.Value,
                },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsOk();
            });

            // 検証のためDBを確認
            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var isExists = await dbContext.Tenants.AnyAsync(t => t.Name == new TenantName(request.Name));
            Assert.True(isExists);
        }

        [Fact(DisplayName = "初期ユーザーのロールがシステム管理者ロールのテナントを作成できる(200)")]
        public async Task CreateTenant_WithSystemAdminRole_ReturnsCreatedTenant()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();
            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "admin@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "田中",
                    FirstName = "太郎",
                    RoleId = _env.SystemAdminRole.Id.Value,
                },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsOk();

                // 検証のためDBを確認
                var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
                var isExists = await dbContext.Tenants.AnyAsync(t => t.Name == new TenantName(request.Name));
                Assert.True(isExists);
            });
        }

        [Fact(DisplayName = "管理者権限はテナントを作成できない(403)")]
        public async Task CreateTenant_AsAdmin_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "admin@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "田中",
                    FirstName = "太郎",
                    RoleId = _env.AdminRole.Id.Value,
                },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "一般ユーザー権限はテナントを作成できない(403)")]
        public async Task CreateTenant_AsGeneralUser_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "user@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "山田",
                    FirstName = "次郎",
                    RoleId = _env.AdminRole.Id.Value,
                },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "認証されていない場合はテナントを作成できない(401)")]
        public async Task CreateTenant_AsUnauthenticated_ReturnsUnauthorized()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "user@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "山田",
                    FirstName = "次郎",
                    RoleId = _env.AdminRole.Id.Value,
                },
            };

            // 認証されていない状態でリクエストを送信
            await _env.Factory.RunWithoutAuthenticationAsync(async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsUnauthorized();
            });
        }

        [Fact(DisplayName = "初期ユーザーのロールが一般ユーザーロールのとき、テナントを作成できない(403)")]
        public async Task CreateTenant_WithGeneralUserRole_ReturnsUnauthorized()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateTenantRequest
            {
                Name = "株式会社AAA",
                InitUser = new()
                {
                    Email = "user@example.aaa.com",
                    Password = "testPassword123",
                    LastName = "山田",
                    FirstName = "次郎",
                    RoleId = _env.AdminRole.Id.Value,
                },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PostAsJsonAsync("/api/tenants", request);

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        #endregion

        #region テナント一覧の取得

        [Fact(DisplayName = "テナント一覧を取得できる(200)")]
        public async Task GetTenants_ReturnsTenantList()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var now = _env.Factory.TimeProvider.GetUtcNow();

            var tenantEms = new TenantEm[]
            {
                TenantEm.Create(TenantId.New(), now, now, null, null, new TenantName("株式会社AAA")),
                TenantEm.Create(TenantId.New(), now, now, null, null, new TenantName("株式会社BBB")),
                TenantEm.Create(TenantId.New(), now, now, null, null, new TenantName("株式会社CCC")),
            };

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            await dbContext.Tenants.AddRangeAsync(tenantEms);
            await dbContext.SaveChangesAsync();

            var claims = _env.GetClaimsForRootUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync("/api/tenants");

                /*** 検証 ***/
                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<List<TenantSummaryResponse>>();
                Assert.NotNull(responseData);

                var expectedTenantEms = new List<TenantEm>
                {
                    _env.DefaultTenant
                };
                expectedTenantEms.AddRange(tenantEms);

                Assert.Equal(expectedTenantEms.Count, responseData.Count);
                Assert.All(responseData, actual =>
                {
                    var expected = expectedTenantEms.FirstOrDefault(x => x.Name.Value == actual.Name);
                    Assert.NotNull(expected);
                    Assert.Equal(expected.Id.Value, actual.Id);
                    Assert.Equal(expected.Name.Value, actual.Name);
                });
            });
        }

        [Fact(DisplayName = "管理者権限でテナント一覧を取得できない(403)")]
        public async Task GetTenants_AsAdmin_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync("/api/tenants");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "一般ユーザー権限でテナント一覧を取得できない(403)")]
        public async Task GetTenants_AsGeneralUser_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync("/api/tenants");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        #endregion

        #region テナントの取得

        [Fact(DisplayName = "テナントを取得できる(200)")]
        public async Task GetTenant_ReturnsTenant()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}");

                /*** 検証 ***/
                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<TenantDetailResponse>();
                Assert.NotNull(responseData);

                Assert.Equal(_env.DefaultTenant.Id.Value, responseData.Id);
                Assert.Equal(_env.DefaultTenant.Name.Value, responseData.Name);
            });
        }

        [Fact(DisplayName = "管理者権限でテナントを取得できない(403)")]
        public async Task GetTenant_AsAdmin_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "一般ユーザー権限でテナントを取得できない(403)")]
        public async Task GetTenant_AsGeneralUser_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "存在しないテナントを取得できない(404)")]
        public async Task GetTenant_WithNotExistsTenantId_ReturnsNotFound()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.GetAsync("/api/tenants/00000000-0000-0000-0000-000000000000");

                /*** 検証 ***/
                response.IsNotFound();
            });
        }

        #endregion

        #region テナントの更新

        [Fact(DisplayName = "テナントを更新できる(200)")]
        public async Task UpdateTenant_ReturnsUpdatedTenant()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();
            var request = new UpdateTenantRequest
            {
                Name = "株式会社DDD",
            };

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PutAsJsonAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}", request);

                /*** 検証 ***/
                response.IsOk();

                var actualTenantEm = await dbContext.Tenants.FindAsync(TenantId.New(_env.DefaultTenant.Id.Value));
                Assert.Equal(request.Name, actualTenantEm?.Name.Value);
            });
        }

        [Fact(DisplayName = "管理者権限でテナントを更新できない(403)")]
        public async Task UpdateTenant_AsAdmin_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var request = new UpdateTenantRequest
            {
                Name = "株式会社DDD",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PutAsJsonAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}", request);

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "一般ユーザー権限でテナントを更新できない(403)")]
        public async Task UpdateTenant_AsGeneralUser_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new UpdateTenantRequest
            {
                Name = "株式会社DDD",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PutAsJsonAsync($"/api/tenants/{_env.DefaultTenant.Id.Value}", request);

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "存在しないテナントを更新できない(400)")]
        public async Task UpdateTenant_WithNotExistsTenantId_ReturnsNotFound()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();
            var request = new UpdateTenantRequest
            {
                Name = "株式会社DDD",
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.PutAsJsonAsync("/api/tenants/00000000-0000-0000-0000-000000000000", request);

                /*** 検証 ***/
                response.IsBadRequest();
            });
        }

        #endregion

        #region テナントの削除

        [Fact(DisplayName = "テナントを削除できる(200)")]
        public async Task DeleteTenant_ReturnsNoContent()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForRootUser();

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var tenantEm = TenantEm.Create(TenantId.New(), _env.Factory.TimeProvider.GetUtcNow(), _env.Factory.TimeProvider.GetUtcNow(), null, null, new TenantName("株式会社AAA"));
            dbContext.Tenants.Add(tenantEm);
            await dbContext.SaveChangesAsync();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.DeleteAsync($"/api/tenants/{tenantEm.Id.Value}");

                /*** 検証 ***/
                response.IsOk();

                var isExists = await dbContext.Tenants.AnyAsync(x => x.Id == tenantEm.Id);
                Assert.False(isExists);
            });
        }

        [Fact(DisplayName = "管理者権限でテナントを削除できない(403)")]
        public async Task DeleteTenant_AsAdmin_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var tenantEm = TenantEm.Create(TenantId.New(), _env.Factory.TimeProvider.GetUtcNow(), _env.Factory.TimeProvider.GetUtcNow(), null, null, new TenantName("株式会社AAA"));
            dbContext.Tenants.Add(tenantEm);
            await dbContext.SaveChangesAsync();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.DeleteAsync($"/api/tenants/{tenantEm.Id.Value}");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "一般ユーザー権限でテナントを削除できない(403)")]
        public async Task DeleteTenant_AsGeneralUser_ReturnsForbidden()
        {
            /*** 準備 ***/
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var tenantEm = TenantEm.Create(TenantId.New(), _env.Factory.TimeProvider.GetUtcNow(), _env.Factory.TimeProvider.GetUtcNow(), null, null, new TenantName("株式会社AAA"));
            dbContext.Tenants.Add(tenantEm);
            await dbContext.SaveChangesAsync();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                /*** 実行 ***/
                var response = await client.DeleteAsync($"/api/tenants/{tenantEm.Id.Value}");

                /*** 検証 ***/
                response.IsForbidden();
            });
        }

        #endregion
    }
}