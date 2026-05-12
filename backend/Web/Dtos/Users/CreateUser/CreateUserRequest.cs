using System.Text.Json.Serialization;

namespace Web.Dtos.Users.CreateUser
{
    /// <summary>
    /// ユーザー作成リクエスト
    /// </summary>
    /// <value></value>
    public record CreateUserRequest
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        /// <value></value>
        [JsonPropertyName("password")]
        public required string Password { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        [JsonPropertyName("lastName")]
        public required string LastName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("firstName")]
        public required string FirstName { get; set; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleId")]
        public required Guid RoleId { get; set; }
    }
}