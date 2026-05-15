using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザー更新ユースケース
    /// </summary>
    public class UpdateUserUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _uow;
        private readonly TimeProvider _timeProvider;
        private readonly IPasswordHashService _passwordHashService;

        public UpdateUserUseCase(IAuthorizeService authorizeService, IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork uow, TimeProvider timeProvider, IPasswordHashService passwordHashService)
        {
            _authorizeService = authorizeService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _uow = uow;
            _timeProvider = timeProvider;
            _passwordHashService = passwordHashService;
        }

        public async Task Execute(TenantId tenantId, UserId actorId, UserId targetId, UpdateUserParam param)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            var actorRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, actorId);
            if (actorRoleEm is null)
            {
                throw new AppValidateException("実行者のロールが不明です。");
            }

            var targetUserEm = await _userRepository.GetByIdAsync(tenantId, targetId, true);
            if (targetUserEm is null)
            {
                throw new AppValidateException("更新対象のユーザーが不明です。");
            }

            // 実行者の権限は更新対象ユーザーのロールと同等以上でないと更新できない
            if (!actorRoleEm.IsHigherOrEqualLevelThan(targetUserEm.Role))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            var now = _timeProvider.GetUtcNow();

            // メールアドレス
            if (!string.IsNullOrWhiteSpace(param.Email))
            {
                var newEmail = new UserEmail(param.Email);
                targetUserEm.ChangeEmail(newEmail, now, actorId);
            }

            // パスワード
            if (!string.IsNullOrWhiteSpace(param.Password))
            {
                var newPasswordhash = _passwordHashService.GenerateHash(param.Password);
                targetUserEm.ChangePassword(newPasswordhash, now, actorId);
            }

            // 氏名
            if (param.LastName != null || param.FirstName != null)
            {
                var newUsername = new UserName(param.LastName ?? targetUserEm.Username.LastName, param.FirstName ?? targetUserEm.Username.FirstName);
                targetUserEm.ChangeName(newUsername, now, actorId);
            }

            // ロール
            if (param.RoleId.HasValue)
            {
                var newRoleId = RoleId.New(param.RoleId.Value);
                var newRoleEm = await _roleRepository.GetByIdAsync(newRoleId);

                if (newRoleEm is null)
                {
                    throw new AppValidateException("指定のロールは不明です。");
                }

                // 実行者よりも強い権限に変更はできない
                if (newRoleEm.IsHigherLevelThan(actorRoleEm))
                {
                    throw new AppForbiddenException("操作は許可されていません。");
                }

                targetUserEm.ChangeRole(newRoleId, now, actorId);
            }

            // 有効フラグ
            if (param.IsActive.HasValue)
            {
                if (param.IsActive.Value)
                {
                    targetUserEm.Activate(now, actorId);
                }
                else
                {
                    targetUserEm.Deactivate(now, actorId);
                }
            }

            // 変更を反映
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ユーザー更新ユースケースのパラメータ
    /// </summary>
    /// <value></value>
    public record UpdateUserParam
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        public string? Email { get; init; }

        /// <summary>
        /// パスワード
        /// </summary>
        /// <value></value>
        public string? Password { get; init; }

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        public string? LastName { get; init; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        public string? FirstName { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        public Guid? RoleId { get; init; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        /// <value></value>
        public bool? IsActive { get; init; }
    }
}