using Application.Repositories;
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

        public async Task<UserEm?> GetByIdAsync(UserId userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserEm?> GetActiveByEmailAsync(UserEmail email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.IsActive);
        }
    }
}