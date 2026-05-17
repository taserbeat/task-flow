using Application.Repositories;
using Application.Services;
using Domain.Entities.Boards;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.Boards
{
    /// <summary>
    /// ボード作成ユースケース
    /// </summary>
    public class CreateBoardUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly TimeProvider _timeProvider;
        private readonly IBoardRepository _boardRepository;
        private readonly IUnitOfWork _uow;

        public CreateBoardUseCase(IAuthorizeService authorizeService, TimeProvider timeProvider, IBoardRepository boardRepository, IUnitOfWork uow)
        {
            _authorizeService = authorizeService;
            _timeProvider = timeProvider;
            _boardRepository = boardRepository;
            _uow = uow;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, CreateBoardParam param)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            var now = _timeProvider.GetUtcNow();

            // パラメータ作成
            var boardEm = BoardEm.Create(
                boardId: BoardId.New(),
                tenantId: tenantId,
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId,
                name: new(param.Name)
            );

            // 登録
            await _boardRepository.AddAsync(boardEm);
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ボードの作成パラメータ
    /// </summary>
    /// <value></value>
    public record CreateBoardParam
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        public required string Name { get; set; }
    }
}