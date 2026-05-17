using Application.Repositories;
using Application.Services;
using Application.UseCases.Users;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Tenants
{
    /// <summary>
    /// テナント作成のユースケース
    /// </summary>
    public class CreateTenantUseCase
    {
        private readonly ILogger<CreateTenantUseCase> _logger;
        private readonly IAuthorizeService _authorizeService;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleService _roleService;
        private readonly TimeProvider _timeProvider;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHashService _passwordHashService;

        public CreateTenantUseCase(ILogger<CreateTenantUseCase> logger, IAuthorizeService authorizeService, IRoleRepository roleRepository, IRoleService roleService, TimeProvider timeProvider, ITenantRepository tenantRepository, IUserRepository userRepository, IUnitOfWork uow, IPasswordHashService passwordHashService)
        {
            _logger = logger;
            _authorizeService = authorizeService;
            _roleRepository = roleRepository;
            _roleService = roleService;
            _timeProvider = timeProvider;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _uow = uow;
            _passwordHashService = passwordHashService;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, CreateTenantParam param)
        {
            // 実行権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.SystemAdmin))
            {
                _logger.LogError($"テナントID: '{tenantId}', ユーザーID: '{actorId}' が許可されていない操作 (テナント作成) を要求したため、拒否しました。");
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 実行者と作成対象ユーザーのロールレベル(強さ)をチェック
            var targetRoleId = RoleId.New(param.InitUserParam.RoleId);
            bool canCreate = await _roleService.CanCreateUserAsync(tenantId, actorId, targetRoleId);
            if (!canCreate)
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // テナントの初期ユーザーはAdmin以上の権限が必要
            var initUserRoleEm = await _roleRepository.GetByIdAsync(targetRoleId);
            var adminRoleName = RoleNameEnum.Admin;
            var adminRoleEm = await _roleRepository.GetByNameAsync(adminRoleName);

            if (initUserRoleEm is null)
            {
                throw new AppValidateException("作成するユーザーのロールが不明です。");
            }

            if (adminRoleEm is null)
            {
                // この例外がスローされたときはAdminロールが存在しない場合であり、
                // DBへの初期データ登録漏れや削除されてしまったことが考えられる。
                _logger.LogError($"'{adminRoleName}' のロール情報が存在しません。");
                throw new InvalidOperationException("");
            }

            if (!initUserRoleEm.IsHigherOrEqualLevelThan(adminRoleEm))
            {
                throw new AppValidateException($"作成するユーザーのロールは '{adminRoleEm.Label.Value}' 以上の権限が必要です。");
            }

            var now = _timeProvider.GetUtcNow();

            // テナント+初期ユーザーの作成処理
            var newTenantEm = TenantEm.Create(
                tenantId: TenantId.New(),
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId,
                name: param.Name
            );

            // パスワード生成
            var passwordhash = _passwordHashService.GenerateHash(param.InitUserParam.Password);

            var initUserEm = UserEm.Create(
                userId: UserId.New(),
                tenantId: newTenantEm.Id,
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId,
                email: new(param.InitUserParam.Email),
                passwordHash: passwordhash,
                username: new(param.InitUserParam.LastName, param.InitUserParam.FirstName),
                roleId: targetRoleId
            );

            // トランザクション
            await _uow.ExecuteTransactionAsync(async () =>
            {
                using var scope = _uow.CreateTenantIdScope(newTenantEm.Id.ToString());

                await _tenantRepository.AddAsync(newTenantEm);
                await _userRepository.AddAsync(initUserEm);
            });
        }
    }

    /// <summary>
    /// テナント作成パラメータ
    /// </summary>
    /// <value></value>
    public record CreateTenantParam
    {
        public required string Name { get; set; }

        public required CreateUserParam InitUserParam { get; set; }
    }
}