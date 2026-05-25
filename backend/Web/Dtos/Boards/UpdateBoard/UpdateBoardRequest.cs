using System.Text.Json.Serialization;

namespace Web.Dtos.Boards.UpdateBoard
{
    /// <summary>
    /// ボードの更新リクエスト
    /// </summary>
    public class UpdateBoardRequest
    {
        /// <summary>
        /// ボード名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}