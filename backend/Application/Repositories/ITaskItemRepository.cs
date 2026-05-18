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
        /// 指定のタスクを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="taskItemId">タスクID</param>
        /// <returns></returns>
        Task<TaskItemEm?> GetByIdAsync(TenantId tenantId, TaskItemId taskItemId);

        /// <summary>
        /// 指定ボード列の最後のタスクの位置番号を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">ボード列ID</param>
        /// <returns></returns>
        Task<TaskItemPosition?> GetLastPositionAsync(TenantId tenantId, BoardColumnId boardColumnId);

        /// <summary>
        /// 指定のタスクを削除する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="taskItemId">タスクID</param>
        /// <returns></returns>
        Task<int> DeleteAsync(TenantId tenantId, TaskItemId taskItemId);
    }
}