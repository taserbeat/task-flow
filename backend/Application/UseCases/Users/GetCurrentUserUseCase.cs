using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users
{
    /// <summary>
    /// 自身のユーザー情報を取得するユースケース
    /// </summary>
    public class GetCurrentUserUseCase
    {
        private readonly ILogger<GetCurrentUserUseCase> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;

        public GetCurrentUserUseCase(ILogger<GetCurrentUserUseCase> logger, ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        public async Task<(TenantEm?, UserEm?)> Execute(TenantId tenantId, UserId userId)
        {
            var tenantEm = await _tenantRepository.GetByIdAsync(tenantId);
            var userEm = await _userRepository.GetByIdAsync(userId, true);

            return (tenantEm, userEm);
        }
    }
}