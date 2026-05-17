using Application.Repositories;
using Application.Services;
using Domain.Entities.Boards;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Exceptions;

namespace Application.UseCases.Boards
{
    /// <summary>
    /// ボードの削除ユースケース
    /// </summary>
    public class DeleteBoardUseCase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IBoardRepository _boardRepository;

        public DeleteBoardUseCase(IAuthorizeService authorizeService, IBoardRepository boardRepository)
        {
            _authorizeService = authorizeService;
            _boardRepository = boardRepository;
        }

        public async Task<int> ExecuteAsync(TenantId tenantId, BoardId boardId)
        {
            // 実行者の権限チェック
            if (!_authorizeService.HasRequiredRole(RoleLevelEnum.Admin))
            {
                throw new AppForbiddenException("操作は許可されていません。");
            }

            return await _boardRepository.DeleteAsync(tenantId, boardId);
        }
    }
}