using Domain.Entities.Common.ValueObjects;
using Domain.Entities.Tenants;

namespace Domain.Entities.Common
{
    /// <summary>
    /// テナントに属する監査可能なエンティティモデルのクラス
    /// </summary>
    public class BaseTenantAuditableEm<TId> : BaseAuditableEm<TId> where TId : struct, IStronglyTypedId<Guid>
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        public TenantId TenantId { get; protected set; }
    }
}