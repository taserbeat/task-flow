using System.Text.Json.Serialization;
using Domain.Entities.Tenants;

namespace Web.Dtos.Tenants.GetTenant
{
    /// <summary>
    /// テナントの詳細レスポンス
    /// </summary>
    public record TenantDetailResponse
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// テナント名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        public static TenantDetailResponse FromEntity(TenantEm em)
        {
            return new TenantDetailResponse
            {
                Id = em.Id.Value,
                Name = em.Name,
            };
        }
    }
}