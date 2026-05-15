using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザー作成のユースケース
    /// </summary>
    public class CreateUserUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IRoleService _roleService;
        private readonly TimeProvider _timeProvider;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;

        public CreateUserUseCase(IAuthorizeService authorizeService, IRoleService roleService, TimeProvider timeProvider, IPasswordHashService passwordHashService, IUserRepository userRepository, IUnitOfWork uow)
        {
            _authorizeService = authorizeService;
            _roleService = roleService;
            _timeProvider = timeProvider;
            _passwordHashService = passwordHashService;
            _userRepository = userRepository;
            _uow = uow;
        }

        public async Task Execute(TenantId tenantId, UserId actorId, CreateUserParam param)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 実行者と作成対象ユーザーのロールレベル(強さ)をチェック
            var targetRoleId = RoleId.New(param.RoleId);
            bool canCreate = await _roleService.CanCreateUserAsync(tenantId, actorId, targetRoleId);
            if (!canCreate)
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // パスワード生成
            var passwordhash = _passwordHashService.GenerateHash(param.Password);

            var now = _timeProvider.GetUtcNow();

            // パラメータ作成 (バリデーション含む)
            var userEm = UserEm.Create(
                userId: UserId.New(),
                tenantId: tenantId,
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId,
                email: new(param.Email),
                passwordHash: passwordhash,
                username: new(param.LastName, param.FirstName),
                roleId: targetRoleId
            );

            // 登録
            await _userRepository.AddAsync(userEm);
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ユーザー作成のパラメータ
    /// </summary>
    /// <value></value>
    public record CreateUserParam
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        public required string Email { get; init; }

        /// <summary>
        /// パスワード
        /// </summary>
        /// <value></value>
        public required string Password { get; init; }

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        public required string LastName { get; init; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        public required string FirstName { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        public required Guid RoleId { get; init; }
    }
}