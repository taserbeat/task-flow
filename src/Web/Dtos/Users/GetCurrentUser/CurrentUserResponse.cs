using System.Text.Json.Serialization;

namespace Web.Dtos.Users.GetCurrentUser
{
    /// <summary>
    /// 自身のユーザー情報
    /// </summary>
    /// <returns></returns>
    public record CurrentUserResponse
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("tenantId")]
        public required Guid TenantId { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("userId")]
        public required Guid UserId { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleName")]
        public required string RoleName { get; set; }

        /// <summary>
        /// ロールレベル
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleLevel")]
        public required int RoleLevel { get; set; }
    }
}