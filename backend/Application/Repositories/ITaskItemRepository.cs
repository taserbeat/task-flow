using Domain.Entities.BoardColumns;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;

namespace Application.Repositories
{
    /// <summary>
    /// タスクリポジトリ
    /// </summary>
    public interface ITaskItemRepository
    {
        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="taskItemEm"></param>
        /// <returns></returns>
        Task AddAsync(TaskItemEm taskItemEm);

        /// <summary>
        /// 指定の列のタスク一覧を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">列ID</param>
        /// <returns></returns>
        Task GetTaskItemsByBoardColumnAsync(TenantId tenantId, BoardColumnId boardColumnId);

        /// <summary>
        /// 指定のタスクを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="taskItemId">タスクID</param>
        /// <returns></returns>
        Task<TaskItemEm?> GetByIdAsync(TenantId tenantId, TaskItemId taskItemId);

        /// <summary>
        /// 指定列の先頭の位置番号を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">列ID</param>
        /// <returns></returns>
        Task<TaskItemPosition?> GetFirstPositionAsync(TenantId tenantId, BoardColumnId boardColumnId);

        /// <summary>
        /// 指定ボード列の最後のタスクの位置番号を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">列ID</param>
        /// <returns></returns>
        Task<TaskItemPosition?> GetLastPositionAsync(TenantId tenantId, BoardColumnId boardColumnId);

        /// <summary>
        /// 指定の位置範囲に含まれるタスクの件数を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">列ID</param>
        /// <param name="low">タスク位置 (小)</param>
        /// <param name="high">タスク位置 (大)</param>
        /// <returns></returns>
        Task<int> CountPositionRangeAsync(TenantId tenantId, BoardColumnId boardColumnId, TaskItemPosition low, TaskItemPosition high);

        /// <summary>
        /// 指定のタスクを削除する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="taskItemId">タスクID</param>
        /// <returns></returns>
        Task<int> DeleteAsync(TenantId tenantId, TaskItemId taskItemId);
    }
}