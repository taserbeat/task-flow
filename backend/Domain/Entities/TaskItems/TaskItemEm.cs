using Domain.Entities.BoardColumns;
using Domain.Entities.Common;
using Domain.Entities.Tenants;
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
        public UserEm? Creator { get; private set; } = default;

        public static TaskItemEm Create(
            TaskItemId id,
            TenantId tenantId,
            BoardColumnId boardColumnId,
            UserId? assigneeId,
            TaskItemTitle title,
            TaskItemDescription description,
            TaskItemPriorityEnum priority,
            DateTimeOffset? dueDate,
            TaskItemPosition position,
            DateTimeOffset createdAt,
            DateTimeOffset updatedAt,
            UserId? createdBy,
            UserId? updatedBy
        )
        {
            return new TaskItemEm()
            {
                Id = id,
                TenantId = tenantId,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
                BoardColumnId = boardColumnId,
                AssigneeId = assigneeId,
                Title = title,
                Description = description,
                Priority = priority,
                DueDate = dueDate,
                Position = position,
            };
        }

        public void ChangeColumn(BoardColumnId newColumnId, TaskItemPosition newPosition, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            BoardColumnId = newColumnId;
            Position = newPosition;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Assign(UserId newAssigneeId, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            AssigneeId = newAssigneeId;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ReleaseAssignee(DateTimeOffset updatedAt, UserId? updatedBy)
        {
            AssigneeId = null;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangeTitle(TaskItemTitle newTitle, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Title = newTitle;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangeDescription(TaskItemDescription newDescription, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Description = newDescription;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangePriority(TaskItemPriorityEnum newPriority, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Priority = newPriority;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangeDueDate(DateTimeOffset newDueDate, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            DueDate = newDueDate;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void DeleteDueDate(DateTimeOffset updatedAt, UserId? updatedBy)
        {
            DueDate = null;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangePosition(TaskItemPosition newPosition, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Position = newPosition;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
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