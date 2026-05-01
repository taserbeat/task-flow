using Domain.Common;

namespace Domain.Entities
{
    /// <summary>
    /// テナントのエンティティモデル
    /// </summary>
    public class TenantEm : BaseAuditableEm
    {
        /// <summary>
        /// テナント名
        /// </summary>
        /// <value></value>
        public required string Name { get; set; }
    }
}