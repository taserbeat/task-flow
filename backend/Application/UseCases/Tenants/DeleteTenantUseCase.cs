using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Tenants
{
    /// <summary>
    /// テナント削除ユースケース
    /// </summary>
    public class DeleteTenantUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly ITenantRepository _tenantRepository;

        public DeleteTenantUseCase(IAuthorizeService authorizeService, ITenantRepository tenantRepository)
        {
            _authorizeService = authorizeService;
            _tenantRepository = tenantRepository;
        }

        public async Task<int> ExecuteAsync(TenantId tenantId, UserId actorId, TenantId targetId)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.SystemAdmin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 自身のテナントは削除できない
            if (targetId == tenantId)
            {
                throw new AppValidateException("自身が所属するテナントは削除できません。");
            }

            return await _tenantRepository.DeleteAsync(targetId);
        }
    }
}