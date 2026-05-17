using System.Text.Json.Serialization;
using Web.Dtos.Users.CreateUser;

namespace Web.Dtos.Tenants.CreateTenant
{
    /// <summary>
    /// テナント作成リクエスト
    /// </summary>
    public record CreateTenantRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("initUser")]
        public required CreateUserRequest InitUser { get; set; }
    }
}