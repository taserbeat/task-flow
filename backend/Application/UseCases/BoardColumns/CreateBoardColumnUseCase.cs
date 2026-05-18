using Application.Repositories;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.BoardColumns
{
    /// <summary>
    /// ボード列の作成ユースケース
    /// </summary>
    public class CreateBoardColumnUseCase
    {
        private readonly TimeProvider _timeProvider;
        private readonly IBoardRepository _boardRepository;
        private readonly IBoardColumnRepository _boardColumnRepository;
        private readonly IUnitOfWork _uow;

        public CreateBoardColumnUseCase(TimeProvider timeProvider, IBoardRepository boardRepository, IBoardColumnRepository boardColumnRepository, IUnitOfWork uow)
        {
            _timeProvider = timeProvider;
            _boardRepository = boardRepository;
            _boardColumnRepository = boardColumnRepository;
            _uow = uow;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, CreateBoardColumnParam param)
        {
            var now = _timeProvider.GetUtcNow();

            // ボードの存在チェック
            var boardId = BoardId.New(param.BoardId);
            var boardEm = await _boardRepository.GetByIdAsync(tenantId, boardId);
            if (boardEm is null)
            {
                throw new AppNotFoundException("指定のボードは存在しません。");
            }

            // ボード列の最後に追加するので、最後の位置を取得
            var lastPosition = await _boardColumnRepository.GetLastPositionAsync(tenantId, boardId);

            // 追加する位置を取得
            var newPosition = lastPosition is null ? BoardColumnPosition.NewInitPosition() : lastPosition.NewNextPosition();

            // パラメータ作成
            var boardColumnEm = BoardColumnEm.Create(
                boardColumnId: BoardColumnId.New(),
                tenantId: tenantId,
                boardId: BoardId.New(param.BoardId),
                name: new(param.Name),
                position: newPosition,
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId
            );

            // 登録
            await _boardColumnRepository.AddAsync(boardColumnEm);
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ボード列の作成パラメータ
    /// </summary>
    /// <value></value>
    public record CreateBoardColumnParam
    {
        /// <summary>
        /// ボードID
        /// </summary>
        /// <value></value>
        public required Guid BoardId { get; set; }

        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        public required string Name { get; set; }
    }
}