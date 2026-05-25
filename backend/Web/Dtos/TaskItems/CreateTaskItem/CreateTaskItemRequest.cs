using System.Text.Json.Serialization;
using Domain.Entities.TaskItems;

namespace Web.Dtos.TaskItems.CreateTaskItem
{
    /// <summary>
    /// タスクの作成リクエスト
    /// </summary>
    public class CreateTaskItemRequest
    {
        /// <summary>
        /// 担当者のユーザーID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("assigneeId")]
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        /// <value></value>
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        /// <value></value>
        [JsonPropertyName("description")]
        public required string Description { get; set; }

        /// <summary>
        /// 優先度
        /// </summary>
        /// <value></value>
        [JsonPropertyName("priority")]
        public required TaskItemPriorityEnum Priority { get; set; }

        /// <summary>
        /// 期限日
        /// </summary>
        /// <value></value>
        [JsonPropertyName("dueDate")]
        public DateTimeOffset? DueDate { get; set; }
    }
}