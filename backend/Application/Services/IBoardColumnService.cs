using Domain.Entities.BoardColumns;
using Domain.Entities.Tenants;

namespace Application.Services
{
    /// <summary>
    /// ボード列のサービス
    /// </summary>
    public interface IBoardColumnService
    {
        /// <summary>
        /// 指定列のタスク位置番号を採番する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardColumnId">列ID</param>
        /// <returns></returns>
        Task RebalanceAsync(TenantId tenantId, BoardColumnId boardColumnId);
    }
}