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
    /// テナントの更新ユースケース
    /// </summary>
    public class UpdateTenantUseCase
    {
        private readonly ILogger<UpdateTenantUseCase> _logger;
        private readonly IAuthorizeService _authorizeService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _uow;
        private readonly TimeProvider _timeProvider;

        public UpdateTenantUseCase(ILogger<UpdateTenantUseCase> logger, IAuthorizeService authorizeService, ITenantRepository tenantRepository, IUnitOfWork uow, TimeProvider timeProvider)
        {
            _logger = logger;
            _authorizeService = authorizeService;
            _tenantRepository = tenantRepository;
            _uow = uow;
            _timeProvider = timeProvider;
        }

        public async Task ExecuteAsync(UserId actorId, TenantId targetId, UpdateTenantParam param)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.SystemAdmin))
            {
                _logger.LogError($"ユーザーID: '{actorId}' が許可されていない操作 (テナントID: '{targetId}' の更新) を要求したため、拒否しました。");
                throw new AppForbiddenException("操作は許可されていません。");
            }

            // 指定のテナントを参照できるようにRLSをバイパスする
            using var scope = _uow.CreateTenantIdScope(targetId.ToString());

            var targetTenantEm = await _tenantRepository.GetByIdAsync(targetId);
            if (targetTenantEm is null)
            {
                throw new AppValidateException("更新対象のテナントが不明です。");
            }

            var now = _timeProvider.GetUtcNow();

            // テナント名
            if (param.Name != null)
            {
                var newTenantName = new TenantName(param.Name);
                targetTenantEm.ChangeName(newTenantName, now, actorId);
            }

            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// テナント更新ユスケースのパラメータ
    /// </summary>
    /// <value></value>
    public record UpdateTenantParam
    {
        public string? Name { get; set; }
    }
}