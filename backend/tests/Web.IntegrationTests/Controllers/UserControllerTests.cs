using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Web.Dtos.Users.CreateUser;
using Web.Dtos.Users.GetCurrentUser;
using Web.Dtos.Users.GetUser;
using Web.Dtos.Users.UpdateUser;
using Web.IntegrationTests.Collections;
using Web.IntegrationTests.Environments;
using Web.IntegrationTests.Extensions;
using Web.IntegrationTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Web.IntegrationTests.Controllers
{
    /// <summary>
    /// UserControllerのインテグレーションテスト
    /// </summary>
    [Collection(CollectionDefinitionNames.Integration)]
    public class UserControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly TestEnvironment _env;

        public UserControllerTests(ITestOutputHelper outputHelper, TestEnvironment env)
        {
            _outputHelper = outputHelper;
            _env = env;
        }

        #region 作成

        [Fact(DisplayName = "管理者権限でユーザーを作成できる(200)")]
        public async Task CreateUser_ReturnsCreatedUser()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var request = new CreateUserRequest
            {
                Email = "newuser@example.com",
                Password = "testPassword123",
                LastName = "山田",
                FirstName = "花子",
                RoleId = _env.UserRole.Id.Value,
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.PostAsJsonAsync("/api/users", request);

                // 検証
                response.IsOk();
            });

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);
            var users = await dbContext.Users.ToListAsync();
            Assert.Contains(users, u => u.Email.Value == request.Email);
        }

        [Fact(DisplayName = "一般ユーザー権限はユーザーを作成できない(403)")]
        public async Task CreateUser_AsGeneralUser_ReturnsForbidden()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();
            var request = new CreateUserRequest
            {
                Email = "newuser@example.com",
                Password = "testPassword123",
                LastName = "山田",
                FirstName = "花子",
                RoleId = _env.UserRole.Id.Value,
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.PostAsJsonAsync("/api/users", request);

                // 検証
                response.IsForbidden();
            });
        }

        #endregion

        #region 取得

        [Fact(DisplayName = "認証されていない場合はユーザー一覧取得で401を返す")]
        public async Task GetUsers_AsUnauthenticated_ReturnsUnauthorized()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();

            await _env.Factory.RunWithoutAuthenticationAsync(async (client) =>
            {
                // 実行
                var response = await client.GetAsync("/api/users");

                // 検証
                response.IsUnauthorized();
            });
        }

        [Fact(DisplayName = "認証ユーザーはユーザー一覧を取得できる(200)")]
        public async Task GetUsers_ReturnsUserList()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();

            var jsonOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.GetAsync("/api/users");

                // 検証
                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<List<UserSummaryResponse>>(jsonOptions);
                Assert.NotNull(responseData);
                Assert.Contains(responseData, x => x.Id == _env.RootUser.Id.Value && x.Email == _env.RootUser.Email.Value);
                Assert.Contains(responseData, x => x.Id == _env.AdminUser.Id.Value && x.Email == _env.AdminUser.Email.Value);
                Assert.Contains(responseData, x => x.Id == _env.SampleUser.Id.Value && x.Email == _env.SampleUser.Email.Value);
            });
        }

        [Fact(DisplayName = "管理者権限でユーザーを取得できる(200)")]
        public async Task GetUser_AsAdmin_ReturnsUserDetail()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var jsonOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.GetAsync($"/api/users/{_env.SampleUser.Id.Value}");

                // 検証
                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<UserDetailResponse>(jsonOptions);
                Assert.NotNull(responseData);
                Assert.Equal(_env.SampleUser.Id.Value, responseData.Id);
                Assert.Equal(_env.SampleUser.Email.Value, responseData.Email);
                Assert.Equal(_env.SampleUser.Username.LastName, responseData.LastName);
                Assert.Equal(_env.SampleUser.Username.FirstName, responseData.FirstName);
                Assert.Equal(_env.UserRole.Name, responseData.Role.Name);
            });
        }

        [Fact(DisplayName = "一般ユーザー権限は特定ユーザーを取得できない(403)")]
        public async Task GetUser_AsGeneralUser_ReturnsForbidden()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.GetAsync($"/api/users/{_env.AdminUser.Id.Value}");

                // 検証
                response.IsForbidden();
            });
        }

        [Fact(DisplayName = "自身のユーザー情報を取得できる(200)")]
        public async Task GetCurrentUser_ReturnsCurrentUser()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForSampleUser();
            var jsonOptions = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
            };

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.GetAsync("/api/users/me");

                // 検証
                response.IsOk();

                var responseData = await response.Content.ReadFromJsonAsync<CurrentUserResponse>(jsonOptions);
                Assert.NotNull(responseData);
                Assert.Equal(_env.DefaultTenant.Id.Value, responseData.Tenant.Id);
                Assert.Equal(_env.DefaultTenant.Name.Value, responseData.Tenant.Name);
                Assert.Equal(_env.SampleUser.Id.Value, responseData.User.Id);
                Assert.Equal(_env.SampleUser.Email.Value, responseData.User.Email);
            });
        }

        #endregion

        #region 更新

        [Fact(DisplayName = "管理者権限でユーザーを更新できる(200)")]
        public async Task UpdateUser_ReturnsUpdatedUser()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var request = new UpdateUserRequest
            {
                Email = "updated@example.com",
                LastName = "更新",
                FirstName = "太郎",
                IsActive = false,
            };

            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.PutAsJsonAsync($"/api/users/{_env.SampleUser.Id.Value}", request);

                // 検証
                response.IsOk();
            });

            var updatedUser = await dbContext.Users.FindAsync(_env.SampleUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(request.Email, updatedUser!.Email.Value);
            Assert.Equal(request.LastName, updatedUser.Username.LastName);
            Assert.Equal(request.FirstName, updatedUser.Username.FirstName);
            Assert.False(updatedUser.IsActive);
        }

        #endregion

        #region 削除

        [Fact(DisplayName = "管理者権限でユーザーを削除できる(200)")]
        public async Task DeleteUser_ReturnsOk()
        {
            // 準備
            await _env.DbFixture.ResetDatabaseAsync();
            await _env.EnsureDefaultDataCreatedAsync();

            var claims = _env.GetClaimsForAdminUser();
            var dbContext = _env.DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            await _env.Factory.RunWithAuthenticationAsync(claims, async (client) =>
            {
                // 実行
                var response = await client.DeleteAsync($"/api/users/{_env.SampleUser.Id.Value}");

                // 検証
                response.IsOk();
            });

            var deletedUser = await dbContext.Users.FindAsync(_env.SampleUser.Id);
            Assert.Null(deletedUser);
        }

        #endregion
    }
}
