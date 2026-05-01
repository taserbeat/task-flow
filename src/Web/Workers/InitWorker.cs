using Application.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Infrastructure.Contexts;

namespace Web.Workers
{
    /// <summary>
    /// 初期化処理を行うワーカー
    /// </summary>
    public class InitWorker : IHostedService
    {
        private readonly ILogger<InitWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public InitWorker(ILogger<InitWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InitWorker started.");

            await EnsureCreatedInitData();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InitWorker stopped.");

            return Task.CompletedTask;
        }

        private static readonly TenantId _rootTenantId = new(Guid.Parse("019de3f2-8964-7fdf-b728-6f2388ffc618"));  // ルートテナント

        private static readonly RoleId _systemAdminRoleId = new(Guid.Parse("019de3fa-9427-7f51-a876-a6030bc90c3f"));  // SystemAdminロール
        private static readonly RoleId _adminRoleId = new(Guid.Parse("019de3fa-9428-74cd-8799-4e6012731f51"));        // Adminロール
        private static readonly RoleId _userRoleId = new(Guid.Parse("019de3fa-9428-7c83-9169-094892f26d4a"));         // userロール

        private static readonly UserId _rootUserId = new(Guid.Parse("019de42e-3e3c-7667-a13d-a0edd99ccd09"));
        private static readonly UserId _adminUserId = new(Guid.Parse("019de42e-3e3d-74df-8781-d584b4150307"));
        private static readonly UserId _sample1UserId = new(Guid.Parse("019de42e-3e3d-7b1b-a9d6-90b0f7d608c2"));

        /// <summary>
        /// 初期データを登録する
        /// </summary>
        /// <returns></returns>
        private async Task EnsureCreatedInitData()
        {
            var now = DateTimeOffset.Parse("2026-05-01T00:09:00+09:00").ToUniversalTime();

            #region テナント

            var rootTenantEm = TenantEm.Create(
                tenantId: _rootTenantId,
                createdAt: now,
                updatedAt: now,
                null,
                null,
                "システム管理用テナント"
            );

            using (var scope = _scopeFactory.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
                var rlsContext = scope.ServiceProvider.GetRequiredService<IRlsContext>();

                using var _ = rlsContext.CreateBypassScope();

                try
                {
                    var savedTenantEm = await tenantRepository.GetByIdAsync(rootTenantEm.Id);
                    if (savedTenantEm is null)
                    {
                        await tenantRepository.AddAsync(rootTenantEm);
                        await uow.SaveChangesAsync();

                        _logger.LogInformation($"テナント: '{rootTenantEm.Name}' を作成しました。");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"テナント: '{rootTenantEm.Name}' の作成に失敗しました。");
                }
            }

            #endregion

            #region ロール

            var roleEms = new RoleEm[]
            {
                RoleEm.Create(
                    roleId: _systemAdminRoleId,
                    name: RoleNameEnum.SystemAdmin
                ),
                RoleEm.Create(
                    roleId: _adminRoleId,
                    name: RoleNameEnum.Admin
                ),
                RoleEm.Create(
                    roleId: _userRoleId,
                    name: RoleNameEnum.User
                ),
            };

            foreach (var roleEm in roleEms)
            {
                using var scope = _scopeFactory.CreateScope();

                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
                var rlsContext = scope.ServiceProvider.GetRequiredService<IRlsContext>();

                using var _ = rlsContext.CreateBypassScope();

                try
                {
                    var savedRoleEm = await roleRepository.GetByIdAsync(roleEm.Id);
                    if (savedRoleEm is null)
                    {
                        await roleRepository.AddAsync(roleEm);
                        await uow.SaveChangesAsync();

                        _logger.LogInformation($"ロール: '{roleEm.Name}' を作成しました。");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ロール: '{roleEm.Name}' の作成に失敗しました。");
                }
            }

            #endregion

            #region ユーザー

            var userEms = new UserEm[]
            {
                UserEm.Create(
                    userId: _rootUserId,
                    tenantId: _rootTenantId,
                    createdAt: now,
                    updatedAt: now,
                    createdBy: null,
                    updatedBy: null,
                    email: new ("root@example.com"),
                    passwordHash: new ("abc123_hashed_password"),
                    roleId: _systemAdminRoleId
                ),
                UserEm.Create(
                    userId: _adminUserId,
                    tenantId: _rootTenantId,
                    createdAt: now,
                    updatedAt: now,
                    createdBy: null,
                    updatedBy: null,
                    email: new ("admin@example.com"),
                    passwordHash: new ("abc123_hashed_password"),
                    roleId: _adminRoleId
                ),
                UserEm.Create(
                    userId: _sample1UserId,
                    tenantId: _rootTenantId,
                    createdAt: now,
                    updatedAt: now,
                    createdBy: null,
                    updatedBy: null,
                    email: new ("sample1@example.com"),
                    passwordHash: new ("abc123_hashed_password"),
                    roleId: _userRoleId
                ),
            };

            foreach (var userEm in userEms)
            {
                using var scope = _scopeFactory.CreateScope();

                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var rlsContext = scope.ServiceProvider.GetRequiredService<IRlsContext>();

                using var _ = rlsContext.CreateBypassScope();

                try
                {
                    var savedUserEm = await userRepository.GetByIdAsync(userEm.Id);
                    if (savedUserEm is null)
                    {
                        // TODO: パスワードハッシュの生成を見直す
                        await userRepository.AddAsync(userEm);
                        await uow.SaveChangesAsync();

                        _logger.LogInformation($"ユーザー: '{userEm.Email}' を作成しました。");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ユーザー: '{userEm.Email}' の作成に失敗しました。");
                }
            }

            #endregion

            return;
        }
    }
}