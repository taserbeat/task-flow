using Application.Repositories;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// ボード列のリポジトリ
    /// </summary>
    public class BoardColumnRepository : IBoardColumnRepository
    {
        private readonly AppDbContext _dbContext;

        public BoardColumnRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(BoardColumnEm boardColumnEm)
        {
            await _dbContext.BoardColumns.AddAsync(boardColumnEm);
        }

        public async Task<IEnumerable<BoardColumnEm>> GetColumnsByBoardAsync(TenantId tenantId, BoardId boardId)
        {
            return await _dbContext.BoardColumns
                .Where(x => x.TenantId == tenantId && x.BoardId == boardId)
                .OrderBy(x => x.Position)
                .ToListAsync();
        }

        public async Task<BoardColumnEm?> GetByIdAsync(TenantId tenantId, BoardColumnId boardColumnId)
        {
            return await _dbContext.BoardColumns
                .Include(bc => bc.TaskItems.OrderBy(t => t.Position))
                .Where(bc => bc.TenantId == tenantId && bc.Id == boardColumnId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByIdAsync(TenantId tenantId, BoardId boardId, BoardColumnId boardColumnId)
        {
            return await _dbContext.BoardColumns.AnyAsync(x => x.TenantId == tenantId && x.BoardId == boardId && x.Id == boardColumnId);
        }

        public async Task<BoardColumnPosition?> GetFirstPositionAsync(TenantId tenantId, BoardId boardId)
        {
            var firstPosition = await _dbContext.BoardColumns
                .Where(bc => bc.TenantId == tenantId && bc.BoardId == boardId)
                .MinAsync(bc => bc.Position);

            return firstPosition;
        }

        public async Task<BoardColumnPosition?> GetLastPositionAsync(TenantId tenantId, BoardId boardId)
        {
            var lastPosition = await _dbContext.BoardColumns
                .Where(bc => bc.TenantId == tenantId && bc.BoardId == boardId)
                .MaxAsync(bc => bc.Position);

            return lastPosition;
        }

        public async Task<int> CountPositionRangeAsync(TenantId tenantId, BoardId boardId, BoardColumnPosition low, BoardColumnPosition high)
        {
            return await _dbContext.BoardColumns
                .Where(x => x.TenantId == tenantId && x.BoardId == boardId && x.Position >= low && x.Position <= high)
                .CountAsync();
        }

        public async Task<int> DeleteAsync(TenantId tenantId, BoardId boardId, BoardColumnId boardColumnId)
        {
            return await _dbContext.BoardColumns
                .Where(x => x.TenantId == tenantId && x.BoardId == boardId && x.Id == boardColumnId)
                .ExecuteDeleteAsync();
        }
    }
}