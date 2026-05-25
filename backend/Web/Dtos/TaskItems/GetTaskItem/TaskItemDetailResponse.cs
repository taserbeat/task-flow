using System.Text.Json.Serialization;
using Domain.Entities.TaskItems;

namespace Web.Dtos.TaskItems.GetTaskItem
{
    /// <summary>
    /// タスクの詳細レスポンス
    /// </summary>
    public class TaskItemDetailResponse
    {
        /// <summary>
        /// タスクID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// 列ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("boardColumnId")]
        public required Guid BoardColumnId { get; set; }

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

        /// <summary>
        /// 位置
        /// </summary>
        /// <value></value>
        [JsonPropertyName("position")]
        public required int Position { get; set; }

        public static TaskItemDetailResponse FromEntity(TaskItemEm em)
        {
            return new TaskItemDetailResponse
            {
                Id = em.Id.Value,
                BoardColumnId = em.BoardColumnId.Value,
                AssigneeId = em.AssigneeId?.Value,
                Title = em.Title.Value,
                Description = em.Description.Value,
                Priority = em.Priority,
                DueDate = em.DueDate,
                Position = em.Position.Value,
            };
        }
    }
}