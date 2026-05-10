using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Common
{
    /// <summary>
    /// エンティティモデルの基底クラス
    /// </summary>
    public abstract class BaseEm<TId> where TId : struct, IStronglyTypedId<Guid>
    {
        /// <summary>
        /// エンティティのID
        /// </summary>
        /// <value></value>
        public TId Id { get; protected set; } = default!;

        protected BaseEm()
        {
        }

        protected BaseEm(TId id)
        {
            Id = id;
        }
    }
}