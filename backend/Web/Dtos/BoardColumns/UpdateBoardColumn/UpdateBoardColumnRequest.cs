using System.Text.Json.Serialization;

namespace Web.Dtos.BoardColumns.UpdateBoardColumn
{
    /// <summary>
    /// ボード列の更新リクエスト
    /// </summary>
    /// <value></value>
    public record UpdateBoardColumnRequest
    {
        /// <summary>
        /// ボード列名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 順番入れ替え後の位置の1つ前のボード列ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("previousColumnId")]
        public Guid? PreviousColumnId { get; set; }

        /// <summary>
        /// 順番入れ替え後の位置の1つ後のボード列ID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("nextColumnId")]
        public Guid? NextColumnId { get; set; }
    }
}