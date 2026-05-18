using Application.Contexts;
using Application.Repositories;
using Application.Services;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Infrastructure.Services
{
    /// <summary>
    /// ボードのサービス
    /// </summary>
    public class BoardService : IBoardService
    {
        private readonly TimeProvider _timeProvider;
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _uow;
        private readonly IBoardColumnRepository _boardColumnRepository;

        public BoardService(TimeProvider timeProvider, IUserContext userContext, IUnitOfWork uow, IBoardColumnRepository boardColumnRepository)
        {
            _timeProvider = timeProvider;
            _userContext = userContext;
            _uow = uow;
            _boardColumnRepository = boardColumnRepository;
        }

        public async Task RebalanceAsync(TenantId tenantId, BoardId boardId)
        {
            var boardColumnEms = await _boardColumnRepository.GetColumnsByBoardAsync(tenantId, boardId);

            var now = _timeProvider.GetUtcNow();

            var nextPosition = BoardColumnPosition.NewInitPosition();
            foreach (var boardColumnEm in boardColumnEms)
            {
                boardColumnEm.ChangePosition(nextPosition, now, _userContext.UserId);
                nextPosition = nextPosition.NewNextPosition();
            }

            await _uow.SaveChangesAsync();
        }
    }
}