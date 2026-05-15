using Application.Repositories;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Application.UseCases.Users
{
    /// <summary>
    /// ユーザーの一覧取得ユースケース
    /// </summary>
    public class GetUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserEm>> Execute(TenantId tenantId)
        {
            return await _userRepository.GetUsersAsync(tenantId);
        }
    }
}