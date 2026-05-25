using Domain.Entities.BoardColumns;
using Domain.Entities.Common;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

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

        public static BoardEm Create(BoardId boardId, TenantId tenantId, BoardName name, DateTimeOffset createdAt, DateTimeOffset updatedAt, UserId? createdBy, UserId? updatedBy)
        {
            return new BoardEm
            {
                Id = boardId,
                TenantId = tenantId,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
                Name = name,
            };
        }

        public void ChangeName(BoardName newName, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Name = newName;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}