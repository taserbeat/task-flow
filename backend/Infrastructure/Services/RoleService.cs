using Application.Repositories;
using Application.Services;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public RoleService(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> CanCreateUserAsync(TenantId tenantId, UserId actorId, RoleId target)
        {
            var actorRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, actorId);
            if (actorRoleEm is null)
            {
                throw new AppValidateException("実行者のロールが不明です。");
            }

            var targetRoleEm = await _roleRepository.GetByIdAsync(target);
            if (targetRoleEm is null)
            {
                throw new AppValidateException("作成するユーザーのロールが不明です。");
            }

            return actorRoleEm.IsHigherOrEqualLevelThan(targetRoleEm);
        }

        public async Task<bool> CanEditUserAsync(TenantId tenantId, UserId actorId, UserId target)
        {
            var actorRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, actorId);
            if (actorRoleEm is null)
            {
                throw new AppValidateException("実行者のロールが不明です。");
            }

            var targetUserRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, target);
            if (targetUserRoleEm is null)
            {
                throw new AppValidateException("対象ユーザーが不明です。");
            }

            return actorRoleEm.IsHigherOrEqualLevelThan(targetUserRoleEm);
        }

        public async Task<bool> CanDeleteUserAsync(TenantId tenantId, UserId actorId, UserId target)
        {
            var actorRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, actorId);
            if (actorRoleEm is null)
            {
                throw new AppValidateException("実行者のロールが不明です。");
            }

            var targetUserRoleEm = await _userRepository.GetRoleByUserIdAsync(tenantId, target);
            if (targetUserRoleEm is null)
            {
                throw new AppValidateException("対象ユーザーが不明です。");
            }

            return actorRoleEm.IsHigherOrEqualLevelThan(targetUserRoleEm);
        }
    }
}