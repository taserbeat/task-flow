using System.Text.Json.Serialization;

namespace Web.Dtos.Users.UpdateUser
{
    /// <summary>
    /// ユーザーの更新リクエスト
    /// </summary>
    public record UpdateUserRequest
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        /// <value></value>
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        [JsonPropertyName("lastName")]
        public string? LastName { get; init; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("firstName")]
        public string? FirstName { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleId")]
        public Guid? RoleId { get; init; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; init; }
    }
}