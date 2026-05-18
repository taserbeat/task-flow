using Application.Repositories;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.UseCases.BoardColumns
{
    /// <summary>
    /// ボード列の削除ユースケース
    /// </summary>
    public class DeleteBoardColumnUseCase
    {
        private readonly IBoardColumnRepository _boardColumnRepository;

        public DeleteBoardColumnUseCase(IBoardColumnRepository boardColumnRepository)
        {
            _boardColumnRepository = boardColumnRepository;
        }

        public async Task ExecuteAsync(TenantId tenantId, BoardId targetBoardId, BoardColumnId targetColumnId)
        {
            await _boardColumnRepository.DeleteAsync(tenantId, targetBoardId, targetColumnId);
        }
    }
}