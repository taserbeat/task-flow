namespace Domain.Common
{
    /// <summary>
    /// 監査可能なエンティティモデルのクラス
    /// </summary>
    public abstract class BaseAuditableEm : BaseEm
    {
        /// <summary>
        /// 作成日時
        /// </summary>
        /// <value></value>
        public required DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        /// <value></value>
        public required Guid? CreatedBy { get; set; }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        /// <value></value>
        public required DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// 最終更新者
        /// </summary>
        /// <value></value>
        public required Guid? UpdatedBy { get; set; }
    }
}