using System.Text.Json.Serialization;
using Domain.Entities.BoardColumns;

namespace Web.Dtos.BoardColumns.GetBoardColumn
{
    /// <summary>
    /// ボード列の詳細レスポンス
    /// </summary>
    public class BoardColumnDetailResponse
    {
        /// <summary>
        /// 列ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// ボードID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("boardId")]
        public required Guid BoardId { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        /// <value></value>
        [JsonPropertyName("position")]
        public required int Position { get; set; }

        public static BoardColumnDetailResponse FromEntity(BoardColumnEm em)
        {
            return new BoardColumnDetailResponse()
            {
                Id = em.Id.Value,
                BoardId = em.BoardId.Value,
                Name = em.Name.Value,
                Position = em.Position.Value,
            };
        }
    }
}