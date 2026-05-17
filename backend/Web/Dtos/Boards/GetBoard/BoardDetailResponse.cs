using System.Text.Json.Serialization;
using Domain.Entities.Boards;

namespace Web.Dtos.Boards.GetBoard
{
    /// <summary>
    /// ボードの詳細情報のレスポンス
    /// </summary>
    public class BoardDetailResponse
    {
        /// <summary>
        /// ボードID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        /// <value></value>
        [JsonPropertyName("createdAt")]
        public required DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        /// <value></value>
        [JsonPropertyName("updatedAt")]
        public required DateTimeOffset UpdatedAt { get; set; }

        public static BoardDetailResponse FromEntity(BoardEm em)
        {
            return new BoardDetailResponse
            {
                Id = em.Id.Value,
                Name = em.Name.Value,
                CreatedAt = em.CreatedAt,
                UpdatedAt = em.UpdatedAt
            };
        }
    }
}