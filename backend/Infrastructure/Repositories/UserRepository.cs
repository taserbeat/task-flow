using Application.Repositories;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Infrastructure.Contexts;
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
        private readonly IRlsContext _rlsContext;

        public UserRepository(AppDbContext dbContext, IRlsContext rlsContext)
        {
            _dbContext = dbContext;
            _rlsContext = rlsContext;
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
                .OrderByDescending(x => x.Email)
                .ToListAsync();
        }

        public async Task<UserEm?> GetByIdAsync(UserId userId, bool isIncludeRole = false)
        {
            var query = _dbContext.Users.AsQueryable();

            if (isIncludeRole)
            {
                query = query.Include(x => x.Role);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserEm?> GetForLoginAsync(UserEmail email)
        {
            // 認証処理ではどんなユーザー情報でも取得できる必要があるため、RLSをバイパスする
            using var _ = _rlsContext.CreateBypassScope();

            var userEm = await _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive);

            return userEm;
        }
    }
}