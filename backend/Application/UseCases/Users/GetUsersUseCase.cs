using Application.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザーの一覧取得ユースケース
    /// </summary>
    public class GetUsersUseCase
    {
        private readonly ILogger<GetUsersUseCase> _logger;
        private readonly IUserRepository _userRepository;

        public GetUsersUseCase(ILogger<GetUsersUseCase> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserEm>> Execute(TenantId tenantId)
        {
            return await _userRepository.GetUsersAsync(tenantId);
        }
    }
}