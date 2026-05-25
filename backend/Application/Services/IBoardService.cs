using Domain.Entities.Boards;
using Domain.Entities.Tenants;

namespace Application.Services
{
    /// <summary>
    /// ボードのサービス
    /// </summary>
    public interface IBoardService
    {
        /// <summary>
        /// 指定ボードの列位置番号を採番する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="boardId">ボードID</param>
        /// <returns></returns>
        Task RebalanceAsync(TenantId tenantId, BoardId boardId);
    }
}