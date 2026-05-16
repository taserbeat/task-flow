using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Exceptions;

namespace Application.UseCases.Tenants
{
    /// <summary>
    /// テナントの取得ユースケース
    /// </summary>
    public class GetTenantUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _uow;

        public GetTenantUseCase(IAuthorizeService authorizeService, ITenantRepository tenantRepository, IUnitOfWork uow)
        {
            _authorizeService = authorizeService;
            _tenantRepository = tenantRepository;
            _uow = uow;
        }

        public async Task<TenantEm?> ExecuteAsync(TenantId targetId)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.SystemAdmin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 指定のテナントを参照できるようにRLSをバイパスする
            using var scope = _uow.CreateTenantIdScope(targetId.ToString());

            return await _tenantRepository.GetByIdAsync(targetId);
        }
    }
}