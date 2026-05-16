using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Tenants
{
    /// <summary>
    /// テナント一覧取得ユースケース
    /// </summary>
    public class GetTenantsUseCase
    {
        private readonly ILogger<GetTenantsUseCase> _logger;
        private readonly IAuthorizeService _authorizeService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _uow;

        public GetTenantsUseCase(ILogger<GetTenantsUseCase> logger, IAuthorizeService authorizeService, ITenantRepository tenantRepository, IUnitOfWork uow)
        {
            _logger = logger;
            _authorizeService = authorizeService;
            _tenantRepository = tenantRepository;
            _uow = uow;
        }

        public async Task<IEnumerable<TenantEm>> ExecuteAsync(TenantId tenantId, UserId actorId)
        {
            // 実行権限チェック
            // 実行権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.SystemAdmin))
            {
                _logger.LogError($"テナントID: '{tenantId}', ユーザーID: '{actorId}' が許可されていない操作 (テナント作成) を要求したため、拒否しました。");
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // テナントを横断して一覧取得するために、RLSをバイパスする
            using var scope = _uow.CreateBypassScope();

            return await _tenantRepository.GetTenantsAsync();
        }
    }
}