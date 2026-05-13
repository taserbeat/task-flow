using System.Text.Json.Serialization;
using Domain.Entities.Roles;
using Domain.Entities.Users;

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
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

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

        /// <summary>
        /// 有効状態
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isActive")]
        public required bool IsActive { get; set; }

        public static UserSummaryResponse FromEntity(UserEm em)
        {
            return new UserSummaryResponse
            {
                Id = em.Id.Value,
                Email = em.Email.Value,
                Username = em.Username.FullName,
                RoleName = em.Role.Name,
                IsActive = em.IsActive,
            };
        }
    }
}