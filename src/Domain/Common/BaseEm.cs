namespace Domain.Common
{
    /// <summary>
    /// エンティティモデルの基底クラス
    /// </summary>
    public abstract class BaseEm
    {
        /// <summary>
        /// エンティティのID
        /// </summary>
        /// <value></value>
        public required Guid Id { get; set; }
    }
}