using Domain.Entities.Boards;
using Domain.Entities.Common;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列のエンティティモデル
    /// </summary>
    public class BoardColumnEm : BaseTenantAuditableEm<BoardColumnId>
    {
        /// <summary>
        /// ボードID
        /// </summary>
        /// <value></value>
        public BoardId BoardId { get; private set; }

        /// <summary>
        /// 列名
        /// </summary>
        /// <value></value>
        public BoardColumnName Name { get; private set; } = default!;

        /// <summary>
        /// 位置
        /// </summary>
        /// <value></value>
        public BoardColumnPosition Position { get; private set; } = default!;

        /// <summary>
        /// ボードのナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public BoardEm Board { get; private set; } = default!;

        /// <summary>
        /// タスク一覧のナビゲーションプロパティ
        /// </summary>
        /// <typeparam name="TaskItemEm"></typeparam>
        /// <returns></returns>
        public ICollection<TaskItemEm> TaskItems { get; private set; } = new List<TaskItemEm>();

        public static BoardColumnEm Create(BoardColumnId boardColumnId, TenantId tenantId, BoardId boardId, BoardColumnName name, BoardColumnPosition position, DateTimeOffset createdAt, DateTimeOffset updatedAt, UserId? createdBy, UserId? updatedBy)
        {
            return new BoardColumnEm()
            {
                Id = boardColumnId,
                TenantId = tenantId,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
                BoardId = boardId,
                Name = name,
                Position = position,
            };
        }

        public void ChangeName(BoardColumnName newName, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Name = newName;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangePosition(BoardColumnPosition newPosition, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Position = newPosition;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}