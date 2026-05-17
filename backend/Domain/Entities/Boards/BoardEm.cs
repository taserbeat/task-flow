using Domain.Entities.BoardColumns;
using Domain.Entities.Common;
using Domain.Entities.Tenants;

namespace Domain.Entities.Boards
{
    /// <summary>
    /// ボードのエンティティモデル
    /// </summary>
    public class BoardEm : BaseTenantAuditableEm<BoardId>
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        public BoardName Name { get; private set; } = default!;

        /// <summary>
        /// ボード列のナビゲーションプロパティ
        /// </summary>
        /// <typeparam name="BoardColumnEm"></typeparam>
        /// <returns></returns>
        public ICollection<BoardColumnEm> Columns { get; private set; } = new List<BoardColumnEm>();

        /// <summary>
        /// テナントのナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public TenantEm Tenant { get; private set; } = default!;
    }
}