using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.Repositories
{
    /// <summary>
    /// ボード列のリポジトリ
    /// </summary>
    public interface IBoardColumnRepository
    {
        /// <summary>
        /// ボード列を追加する
        /// </summary>
        /// <param name="boardColumnEm"></param>
        /// <returns></returns>
        Task AddAsync(BoardColumnEm boardColumnEm);

        /// <summary>
        /// 指定のボード列を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">ボード列ID</param>
        /// <returns></returns>
        Task<BoardColumnEm?> GetByIdAsync(TenantId tenantId, BoardColumnId boardColumnId);

        /// <summary>
        /// 指定ボードの最終列の位置番号を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardId">ボードID</param>
        /// <returns></returns>
        Task<BoardColumnPosition?> GetLastPositionAsync(TenantId tenantId, BoardId boardId);

        /// <summary>
        /// 指定のボード列を削除する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">ボード列ID</param>
        /// <returns></returns>
        Task<int> DeleteAsync(TenantId tenantId, BoardColumnId boardColumnId);
    }
}