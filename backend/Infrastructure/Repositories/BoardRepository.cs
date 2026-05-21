using Application.Repositories;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// ボードのリポジトリ
    /// </summary>
    public class BoardRepository : IBoardRepository
    {
        private readonly AppDbContext _dbContext;

        public BoardRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(BoardEm boardEm)
        {
            await _dbContext.Boards.AddAsync(boardEm);
        }

        public async Task<IEnumerable<BoardEm>> GetBoardsAsync(TenantId tenantId)
        {
            return await _dbContext.Boards
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public Task<BoardEm?> GetByIdAsync(TenantId tenantId, BoardId boardId)
        {
            // TODO:
            // IncludeよりもSelectを使ったプロジェクションの方が軽いので、変更する。
            // (プロジェクション用のデータモデルクラスが必要)

            return _dbContext.Boards
                .Include(x => x.Columns.OrderBy(bc => bc.Position))
                .ThenInclude(bc => bc.TaskItems.OrderBy(t => t.Position))
                .Where(x => x.TenantId == tenantId && x.Id == boardId)
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsByIdAsync(TenantId tenantId, BoardId boardId)
        {
            return _dbContext.Boards.AnyAsync(x => x.TenantId == tenantId && x.Id == boardId);
        }

        public async Task<int> DeleteAsync(TenantId tenantId, BoardId boardId)
        {
            return await _dbContext.Boards
                .Where(x => x.TenantId == tenantId && x.Id == boardId)
                .ExecuteDeleteAsync();
        }
    }
}