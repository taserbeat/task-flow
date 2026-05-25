using System.Text.Json.Serialization;
using Domain.Entities.Tenants;

namespace Web.Dtos.Tenants.GetTenant
{
    /// <summary>
    /// テナントのサマリーレスポンス
    /// </summary>
    /// <value></value>
    public record TenantSummaryResponse
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        /// <value></value>
        [JsonPropertyName("createdAt")]
        public required DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        /// <value></value>
        [JsonPropertyName("updatedAt")]
        public required DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// テナント名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        public static TenantSummaryResponse FromEntity(TenantEm em)
        {
            return new TenantSummaryResponse
            {
                Id = em.Id.Value,
                Name = em.Name.Value,
                CreatedAt = em.CreatedAt,
                UpdatedAt = em.UpdatedAt,
            };
        }
    }
}