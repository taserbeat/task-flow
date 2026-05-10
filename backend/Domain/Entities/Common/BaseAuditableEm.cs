using Domain.Entities.Common.ValueObjects;
using Domain.Entities.Users;

namespace Domain.Entities.Common
{
    /// <summary>
    /// 監査可能なエンティティモデルのクラス
    /// </summary>
    public abstract class BaseAuditableEm<TId> : BaseEm<TId> where TId : struct, IStronglyTypedId<Guid>
    {
        /// <summary>
        /// 作成日時
        /// </summary>
        /// <value></value>
        public DateTimeOffset CreatedAt { get; protected set; }

        /// <summary>
        /// 作成者
        /// </summary>
        /// <value></value>
        public UserId? CreatedBy { get; protected set; }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        /// <value></value>
        public DateTimeOffset UpdatedAt { get; protected set; }

        /// <summary>
        /// 最終更新者
        /// </summary>
        /// <value></value>
        public UserId? UpdatedBy { get; protected set; }

        protected void SetCreated(DateTimeOffset createdAt, UserId? createdBy)
        {
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        protected void SetUpdated(DateTimeOffset updatedAt, UserId? updatedBy)
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}