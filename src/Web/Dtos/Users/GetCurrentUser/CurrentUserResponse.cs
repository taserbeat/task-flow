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
        public Guid TenantId { get; init; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("userId")]
        public Guid UserId { get; init; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleName")]
        public string RoleName { get; init; } = string.Empty;

        /// <summary>
        /// ロールレベル
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleLevel")]
        public int RoleLevel { get; init; }
    }
}