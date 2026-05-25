using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.Repositories
{
    /// <summary>
    /// ボードのリポジトリ
    /// </summary>
    public interface IBoardRepository
    {
        /// <summary>
        /// ボードを追加する
        /// </summary>
        /// <param name="boardEm"></param>
        /// <returns></returns>
        Task AddAsync(BoardEm boardEm);

        /// <summary>
        /// ボードの一覧を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <returns></returns>
        Task<IEnumerable<BoardEm>> GetBoardsAsync(TenantId tenantId);

        /// <summary>
        /// 指定のボードを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardId">ボードID</param>
        /// <returns></returns>
        Task<BoardEm?> GetByIdAsync(TenantId tenantId, BoardId boardId);

        /// <summary>
        /// 指定のボードが存在するか取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardId">ボードID</param>
        /// <returns></returns>
        Task<bool> ExistsByIdAsync(TenantId tenantId, BoardId boardId);

        /// <summary>
        /// 指定のボードを削除する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardId">ボードID</param>
        /// <returns></returns>
        Task<int> DeleteAsync(TenantId tenantId, BoardId boardId);
    }
}