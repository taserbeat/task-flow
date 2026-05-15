using System.Text.Json.Serialization;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Users.GetUser;

namespace Web.Dtos.Users.GetCurrentUser
{
    /// <summary>
    /// 自身のユーザー情報
    /// </summary>
    /// <returns></returns>
    public record CurrentUserResponse
    {
        /// <summary>
        /// テナント
        /// </summary>
        /// <value></value>
        [JsonPropertyName("tenant")]
        public required TenantDetailResponse Tenant { get; set; }

        /// <summary>
        /// ユーザー
        /// </summary>
        /// <value></value>
        [JsonPropertyName("user")]
        public required UserSummaryResponse User { get; set; }
    }
}