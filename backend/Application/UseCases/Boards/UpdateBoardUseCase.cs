using Application.Repositories;
using Application.Services;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Boards
{
    /// <summary>
    /// ボードの更新ユースケース
    /// </summary>
    public class UpdateBoardUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly TimeProvider _timeProvider;
        private readonly IBoardRepository _boardRepository;
        private readonly IUnitOfWork _uow;

        public UpdateBoardUseCase(IAuthorizeService authorizeService, TimeProvider timeProvider, IBoardRepository boardRepository, IUnitOfWork uow)
        {
            _authorizeService = authorizeService;
            _timeProvider = timeProvider;
            _boardRepository = boardRepository;
            _uow = uow;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, BoardId boardId, UpdateBoardParam param)
        {
            var boardEm = await _boardRepository.GetByIdAsync(tenantId, boardId);
            if (boardEm is null)
            {
                throw new AppNotFoundException("指定のボードは不明です。");
            }

            var now = _timeProvider.GetUtcNow();

            // ボード名
            if (!string.IsNullOrWhiteSpace(param.Name))
            {
                var newName = new BoardName(param.Name!);
                boardEm.ChangeName(newName, now, actorId);
            }

            // 変更を反映
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ボードの更新パラメータ
    /// </summary>
    /// <value></value>
    public record UpdateBoardParam
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        public string? Name { get; init; }
    }
}