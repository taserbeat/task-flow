using Application.Repositories;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// ユーザーのリポジトリ
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserEm userEm)
        {
            await _dbContext.Users.AddAsync(userEm);
        }

        public async Task<IEnumerable<UserEm>> GetUsersAsync(TenantId tenantId)
        {
            return await _dbContext.Users
                .Include(x => x.Role)
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.Role.Level)
                .ThenBy(x => x.Email)
                .ToListAsync();
        }

        public async Task<UserEm?> GetByIdAsync(TenantId tenantId, UserId userId, bool isIncludeRole = false)
        {
            var query = _dbContext.Users.Where(x => x.TenantId == tenantId && x.Id == userId);

            if (isIncludeRole)
            {
                query = query.Include(x => x.Role);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<UserEm?> GetByEmailAsync(UserEmail email)
        {
            var userEm = await _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive);

            return userEm;
        }

        public async Task<RoleEm?> GetRoleByUserIdAsync(TenantId tenantId, UserId userId)
        {
            var userEm = await _dbContext.Users
                .Where(x => x.TenantId == tenantId && x.Id == userId)
                .Include(x => x.Role)
                .FirstOrDefaultAsync();

            return userEm?.Role;
        }

        public async Task<bool> ExistsByIdAsync(TenantId tenantId, UserId userId)
        {
            return await _dbContext.Users.AnyAsync(x => x.TenantId == tenantId && x.Id == userId);
        }

        public async Task<int> DeleteAsync(TenantId tenantId, UserId userId)
        {
            return await _dbContext.Users
                .Where(x => x.TenantId == tenantId && x.Id == userId)
                .ExecuteDeleteAsync();
        }
    }
}