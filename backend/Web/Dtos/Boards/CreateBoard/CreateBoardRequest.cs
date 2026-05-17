using System.Text.Json.Serialization;

namespace Web.Dtos.Boards.CreateBoard
{
    /// <summary>
    /// ボード作成リクエスト
    /// </summary>
    /// <value></value>
    public record CreateBoardRequest
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}