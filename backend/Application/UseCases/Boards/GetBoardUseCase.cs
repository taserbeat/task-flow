using Application.Repositories;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.UseCases.Boards
{
    /// <summary>
    /// ボードの詳細情報取得ユースケース
    /// </summary>
    public class GetBoardUseCase
    {
        private readonly IBoardRepository _boardRepository;

        public GetBoardUseCase(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<BoardEm?> ExecuteAsync(TenantId tenantId, BoardId boardId)
        {
            return await _boardRepository.GetByIdAsync(tenantId, boardId);
        }
    }
}