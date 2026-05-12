using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザー削除のユースケース
    /// </summary>
    public class DeleteUserUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;

        public DeleteUserUseCase(IAuthorizeService authorizeService, IUserRepository userRepository, IRoleService roleService)
        {
            _authorizeService = authorizeService;
            _userRepository = userRepository;
            _roleService = roleService;
        }

        public async Task<int> Execute(TenantId tenantId, UserId actorId, UserId targetId)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 自身は削除できない
            if (actorId == targetId)
            {
                throw new AppValidateException("自身を削除することはできません。");
            }

            // 実行者と削除対象ユーザーのロールレベル(強さ)をチェック
            bool canDelete = await _roleService.CanDeleteUserAsync(tenantId, actorId, targetId);
            if (!canDelete)
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            return await _userRepository.DeleteAsync(tenantId, targetId);
        }
    }
}