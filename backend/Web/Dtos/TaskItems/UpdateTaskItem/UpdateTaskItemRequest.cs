using System.Text.Json.Serialization;
using Domain.Entities.TaskItems;

namespace Web.Dtos.TaskItems.UpdateTaskItem
{
    /// <summary>
    /// タスクの更新リクエスト
    /// </summary>
    public class UpdateTaskItemRequest
    {
        /// <summary>
        /// 変更後の列ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("boardColumnId")]
        public Guid? BoardColumnId { get; set; }

        /// <summary>
        /// 変更後の担当者ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("assigneeId")]
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// 担当者を解除するか?
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isReleaseAssignee")]
        public bool? IsReleaseAssignee { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        /// <value></value>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        /// <value></value>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 優先度
        /// </summary>
        /// <value></value>
        [JsonPropertyName("priority")]
        public TaskItemPriorityEnum? Priority { get; set; }

        /// <summary>
        /// 変更後の期限日
        /// </summary>
        /// <value></value>
        [JsonPropertyName("dueDate")]
        public DateTimeOffset? DueDate { get; set; }

        /// <summary>
        /// 期限日を削除するか?
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isDeleteDueDate")]
        public bool? IsDeleteDueDate { get; set; }

        /// <summary>
        /// 変更後の位置の1つ前のタスクID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        [JsonPropertyName("previousTaskItemId")]
        public Guid? PreviousTaskItemId { get; set; }

        /// <summary>
        /// 変更後の位置の1つ後のタスクID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        [JsonPropertyName("nextTaskItemId")]
        public Guid? NextTaskItemId { get; set; }
    }
}