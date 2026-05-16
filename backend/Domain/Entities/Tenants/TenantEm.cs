using Domain.Entities.Common;
using Domain.Entities.Users;

namespace Domain.Entities.Tenants
{
    /// <summary>
    /// テナントのエンティティモデル
    /// </summary>
    public class TenantEm : BaseAuditableEm<TenantId>
    {
        /// <summary>
        /// テナント名
        /// </summary>
        /// <value></value>
        public string Name { get; protected set; } = default!;

        public void ChangeName(string newName, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Name = newName;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public static TenantEm Create(TenantId tenantId, DateTimeOffset createdAt, DateTimeOffset updatedAt, UserId? createdBy, UserId? updatedBy, string name)
        {
            return new TenantEm
            {
                Id = tenantId,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
                Name = name,
            };
        }
    }
}