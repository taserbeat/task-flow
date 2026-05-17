using Domain.Entities.BoardColumns;
using Domain.Entities.Common;
using Domain.Entities.Users;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクのエンティティモデル
    /// </summary>
    public class TaskItemEm : BaseTenantAuditableEm<TaskItemId>
    {
        /// <summary>
        /// ボード列ID
        /// </summary>
        /// <value></value>
        public BoardColumnId BoardColumnId { get; private set; }

        /// <summary>
        /// 担当者ID
        /// </summary>
        /// <value></value>
        public UserId? AssigneeId { get; private set; }

        /// <summary>
        /// タイトル
        /// </summary>
        /// <value></value>
        public TaskItemTitle Title { get; private set; } = default!;

        /// <summary>
        /// 説明
        /// </summary>
        /// <value></value>
        public TaskItemDescription Description { get; private set; } = default!;

        /// <summary>
        /// 優先度
        /// </summary>
        /// <value></value>
        public TaskItemPriorityEnum Priority { get; private set; }

        /// <summary>
        /// 期限日
        /// </summary>
        /// <value></value>
        public DateTimeOffset? DueDate { get; private set; }

        /// <summary>
        /// 位置
        /// </summary>
        /// <value></value>
        public TaskItemPosition Position { get; private set; } = default!;

        /// <summary>
        /// ボード列のナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public BoardColumnEm Column { get; private set; } = default!;

        /// <summary>
        /// 担当者のナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public UserEm? Assignee { get; private set; }

        /// <summary>
        /// 作成者のナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public UserEm? Creator { get; private set; } = default!;
    }

    /// <summary>
    /// タスクの優先度を表すenum
    /// </summary>
    public enum TaskItemPriorityEnum
    {
        /// <summary>
        /// 優先度 低
        /// </summary>
        Low = 0,

        /// <summary>
        /// 優先度 中
        /// </summary>
        Medium = 1,

        /// <summary>
        /// 優先度 高
        /// </summary>
        High = 2,
    }
}