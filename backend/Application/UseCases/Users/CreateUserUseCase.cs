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

        public async Task Execute(CreateUserInput input)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 実行者と作成対象ユーザーのロールレベル(強さ)をチェック
            var targetRoleId = RoleId.New(input.RoleId);
            bool canCreate = await _roleService.CanCreateUserAsync(input.TenantId, input.ActorId, targetRoleId);
            if (!canCreate)
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // パスワード生成
            var passwordhash = _passwordHashService.GenerateHash(input.Password);

            var now = _timeProvider.GetUtcNow();

            // パラメータ作成 (バリデーション含む)
            var userEm = UserEm.Create(
                userId: UserId.New(),
                tenantId: input.TenantId,
                createdAt: now,
                updatedAt: now,
                createdBy: input.ActorId,
                updatedBy: input.ActorId,
                email: new(input.Email),
                passwordHash: passwordhash,
                username: new(input.LastName, input.FirstName),
                roleId: targetRoleId
            );

            // 登録
            await _userRepository.AddAsync(userEm);
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ユーザー作成ユースケースへの入力情報
    /// </summary>
    /// <value></value>
    public record CreateUserInput
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        public required TenantId TenantId { get; init; }

        /// <summary>
        /// 実行者のユーザーID
        /// </summary>
        /// <value></value>
        public required UserId ActorId { get; init; }

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