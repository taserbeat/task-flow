using System.Text.Json.Serialization;
using Domain.Entities.Roles;

namespace Web.Dtos.Users.GetUser
{
    /// <summary>
    /// ユーザー情報
    /// </summary>
    /// <value></value>
    public record UserSummaryResponse
    {
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
        /// 氏名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("username")]
        public required string Username { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("roleName")]
        public required RoleNameEnum RoleName { get; set; }
    }
}