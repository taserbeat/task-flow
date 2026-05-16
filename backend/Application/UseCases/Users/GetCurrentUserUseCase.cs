using Application.Repositories;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Application.UseCases.Users
{
    /// <summary>
    /// 自身のユーザー情報を取得するユースケース
    /// </summary>
    public class GetCurrentUserUseCase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;

        public GetCurrentUserUseCase(ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public async Task<(TenantEm?, UserEm?)> ExecuteAsync(TenantId tenantId, UserId userId)
        {
            var tenantEm = await _tenantRepository.GetByIdAsync(tenantId);
            var userEm = await _userRepository.GetByIdAsync(tenantId, userId, true);

            return (tenantEm, userEm);
        }
    }
}