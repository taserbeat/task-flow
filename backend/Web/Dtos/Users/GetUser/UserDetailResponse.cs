using System.Text.Json.Serialization;
using Domain.Entities.Users;
using Web.Dtos.Roles;

namespace Web.Dtos.Users.GetUser
{
    /// <summary>
    /// ユーザーの詳細レスポンス
    /// </summary>
    public class UserDetailResponse
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
        /// ロール
        /// </summary>
        /// <value></value>
        [JsonPropertyName("role")]
        public required RoleDetailResponse Role { get; set; }

        public static UserDetailResponse FromEntity(UserEm em)
        {
            return new UserDetailResponse
            {
                Id = em.Id.Value,
                Email = em.Email.Value,
                Username = em.Username.FullName,
                Role = RoleDetailResponse.FromEntity(em.Role)
            };
        }
    }
}