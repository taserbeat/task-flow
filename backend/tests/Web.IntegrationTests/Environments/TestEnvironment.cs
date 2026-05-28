using System.Security.Claims;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Web.Common.Constants;
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

        private readonly DateTimeOffset _fixedNow = DateTimeOffset.Parse("2026-05-01T00:09:00+09:00").ToUniversalTime();

        public TenantEm DefaultTenant => TenantEm.Create(
            tenantId: TenantId.New(Guid.Parse("019de3f2-8964-7fdf-b728-6f2388ffc618")),
            createdAt: _fixedNow,
            updatedAt: _fixedNow,
            createdBy: null,
            updatedBy: null,
            name: new("システム管理用テナント")
        );

        public RoleEm SystemAdminRole => RoleEm.Create(
            roleId: RoleId.New(Guid.Parse("019de3fa-9427-7f51-a876-a6030bc90c3f")),
            name: RoleNameEnum.SystemAdmin,
            label: new RoleLabel("システム管理者"),
            level: RoleLevelEnum.SystemAdmin
        );

        public RoleEm AdminRole => RoleEm.Create(
            roleId: RoleId.New(Guid.Parse("019de3fa-9428-74cd-8799-4e6012731f51")),
            name: RoleNameEnum.Admin,
            label: new RoleLabel("管理者"),
            level: RoleLevelEnum.Admin
        );

        public RoleEm UserRole => RoleEm.Create(
            roleId: RoleId.New(Guid.Parse("019de3fa-9428-7c83-9169-094892f26d4a")),
            name: RoleNameEnum.User,
            label: new RoleLabel("ユーザー"),
            level: RoleLevelEnum.User
        );

        public UserEm RootUser => UserEm.Create(
            userId: UserId.New(Guid.Parse("019de42e-3e3c-7667-a13d-a0edd99ccd09")),
            tenantId: DefaultTenant.Id,
            createdAt: _fixedNow,
            updatedAt: _fixedNow,
            createdBy: null,
            updatedBy: null,
            email: new("root@example.com"),
            passwordHash: new("hashedpassword"),  // 本来はパスワードはハッシュ化された値を使用
            username: new("[System]", ""),
            roleId: SystemAdminRole.Id
        );

        public UserEm AdminUser => UserEm.Create(
            userId: UserId.New(Guid.Parse("019de42e-3e3d-74df-8781-d584b4150307")),
            tenantId: DefaultTenant.Id,
            createdAt: _fixedNow,
            updatedAt: _fixedNow,
            createdBy: null,
            updatedBy: null,
            email: new("admin@example.com"),
            passwordHash: new("hashedpassword"),  // 本来はパスワードはハッシュ化された値を使用
            username: new("管理", "太郎"),
            roleId: AdminRole.Id
        );

        public UserEm SampleUser => UserEm.Create(
            userId: UserId.New(Guid.Parse("019de42e-3e3d-7b1b-a9d6-90b0f7d608c2")),
            tenantId: DefaultTenant.Id,
            createdAt: _fixedNow,
            updatedAt: _fixedNow,
            createdBy: null,
            updatedBy: null,
            email: new("sample@example.com"),
            passwordHash: new("hashedpassword"),  // 本来はパスワードはハッシュ化された値を使用
            username: new("ユーザー", "花子"),
            roleId: UserRole.Id
        );

        /// <summary>
        /// テスト用のデフォルトテナントを作成した状態にする
        /// </summary>
        /// <returns></returns>
        public async Task EnsureDefaultTenantCreatedAsync()
        {
            using var db = DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            var now = DateTimeOffset.Parse("2026-05-01T00:09:00+09:00").ToUniversalTime();

            var tenantEm = DefaultTenant;

            var existingTenantEm = await db.Tenants.FindAsync(tenantEm.Id);
            if (existingTenantEm is null)
            {
                db.Tenants.Add(tenantEm);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// テスト用のデフォルトロールを作成した状態にする
        /// </summary>
        /// <returns></returns>
        public async Task EnsureDefaultRolesCreatedAsync()
        {
            using var db = DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            var roles = new[] { SystemAdminRole, AdminRole, UserRole };

            foreach (var role in roles)
            {
                var existingRoleEm = await db.Roles.FindAsync(role.Id);
                if (existingRoleEm is null)
                {
                    db.Roles.Add(role);
                }
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// テスト用のデフォルトユーザーを作成した状態にする
        /// </summary>
        /// <returns></returns>
        public async Task EnsureDefaultUsersCreatedAsync()
        {
            using var db = DbFixture.CreateDbContext(TestDbConnectionType.Migrate);

            var users = new[] { RootUser, AdminUser, SampleUser };

            foreach (var user in users)
            {
                var existingUserEm = await db.Users.FindAsync(user.Id);
                if (existingUserEm is null)
                {
                    db.Users.Add(user);
                }
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// テスト用のデフォルトデータを作成した状態にする
        /// </summary>
        /// <returns></returns>
        public async Task EnsureDefaultDataCreatedAsync()
        {
            // テナント
            await EnsureDefaultTenantCreatedAsync();

            // ロール
            await EnsureDefaultRolesCreatedAsync();

            // ユーザー
            await EnsureDefaultUsersCreatedAsync();
        }

        /// <summary>
        /// システム管理者ユーザーのクレームを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaimsForRootUser()
        {
            return
            [
                new Claim(CustomClaimTypes.TenantId, RootUser.TenantId.ToString()),
                new Claim(CustomClaimTypes.UserId, RootUser.Id.ToString()),
                new Claim(CustomClaimTypes.Email, RootUser.Email.Value),
                new Claim(CustomClaimTypes.RoleId, SystemAdminRole.Id.ToString()),
                new Claim(CustomClaimTypes.RoleName, SystemAdminRole.Name.ToString()),
                new Claim(CustomClaimTypes.RoleLevel, SystemAdminRole.Level.ToString()),
            ];
        }

        /// <summary>
        /// 管理ユーザーのクレームを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaimsForAdminUser()
        {
            return
            [
                new Claim(CustomClaimTypes.TenantId, AdminUser.TenantId.ToString()),
                new Claim(CustomClaimTypes.UserId, AdminUser.Id.ToString()),
                new Claim(CustomClaimTypes.Email, AdminUser.Email.Value),
                new Claim(CustomClaimTypes.RoleId, AdminRole.Id.ToString()),
                new Claim(CustomClaimTypes.RoleName, AdminRole.Name.ToString()),
                new Claim(CustomClaimTypes.RoleLevel, AdminRole.Level.ToString()),
            ];
        }

        /// <summary>
        /// サンプルユーザーのクレームを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Claim> GetClaimsForSampleUser()
        {
            return
            [
                new Claim(CustomClaimTypes.TenantId, SampleUser.TenantId.ToString()),
                new Claim(CustomClaimTypes.UserId, SampleUser.Id.ToString()),
                new Claim(CustomClaimTypes.Email, SampleUser.Email.Value),
                new Claim(CustomClaimTypes.RoleId, UserRole.Id.ToString()),
                new Claim(CustomClaimTypes.RoleName, UserRole.Name.ToString()),
                new Claim(CustomClaimTypes.RoleLevel, UserRole.Level.ToString()),
            ];
        }
    }
}