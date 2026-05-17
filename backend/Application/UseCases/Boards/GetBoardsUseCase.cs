using Application.Repositories;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.UseCases.Boards
{
    /// <summary>
    /// ボードの一覧取得ユースケース
    /// </summary>
    public class GetBoardsUseCase
    {
        private readonly IBoardRepository _boardRepository;

        public GetBoardsUseCase(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<IEnumerable<BoardEm>> ExecuteAsync(TenantId tenantId)
        {
            var boardsEms = await _boardRepository.GetBoardsAsync(tenantId);

            return boardsEms;
        }
    }
}