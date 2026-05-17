using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザーの取得ユースケース
    /// </summary>
    public class GetUserUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _userRepository;

        public GetUserUseCase(IAuthorizeService authorizeService, IUserRepository userRepository)
        {
            _authorizeService = authorizeService;
            _userRepository = userRepository;
        }

        public async Task<UserEm?> ExecuteAsync(TenantId tenantId, UserId targetId)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            return await _userRepository.GetByIdAsync(tenantId, targetId, true);
        }
    }
}