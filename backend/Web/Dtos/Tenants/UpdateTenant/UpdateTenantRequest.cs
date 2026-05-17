using System.Text.Json.Serialization;

namespace Web.Dtos.Tenants.UpdateTenant
{
    /// <summary>
    /// テナントの更新リクエスト
    /// </summary>
    /// <value></value>
    public record UpdateTenantRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}