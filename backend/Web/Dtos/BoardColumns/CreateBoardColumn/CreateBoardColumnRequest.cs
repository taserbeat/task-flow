using System.Text.Json.Serialization;

namespace Web.Dtos.BoardColumns.CreateBoardColumn
{
    /// <summary>
    /// ボード列の作成リクエスト
    /// </summary>
    /// <value></value>
    public record CreateBoardColumnRequest
    {
        /// <summary>
        /// ボード列名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}