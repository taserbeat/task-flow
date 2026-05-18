using Application.Repositories;
using Domain.Entities.BoardColumns;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// タスクリポジトリ
    /// </summary>
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskItemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TaskItemEm taskItemEm)
        {
            await _dbContext.TaskItems.AddAsync(taskItemEm);
        }

        public Task<TaskItemEm?> GetByIdAsync(TenantId tenantId, TaskItemId taskItemId)
        {
            return _dbContext.TaskItems
                .Where(x => x.TenantId == tenantId && x.Id == taskItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<TaskItemPosition?> GetLastPositionAsync(TenantId tenantId, BoardColumnId boardColumnId)
        {
            var lastPosition = await _dbContext.TaskItems
                .Where(x => x.TenantId == tenantId && x.BoardColumnId == boardColumnId)
                .MaxAsync(t => t.Position);

            return lastPosition;
        }

        public async Task<int> DeleteAsync(TenantId tenantId, TaskItemId taskItemId)
        {
            return await _dbContext.TaskItems
                .Where(x => x.TenantId == tenantId && x.Id == taskItemId)
                .ExecuteDeleteAsync();
        }
    }
}