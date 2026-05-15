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
        /// 有効状態
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isActive")]
        public required bool IsActive { get; set; }

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
                LastName = em.Username.LastName,
                FirstName = em.Username.FirstName,
                IsActive = em.IsActive,
                Role = RoleDetailResponse.FromEntity(em.Role)
            };
        }
    }
}