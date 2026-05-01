namespace Application.Contexts
{
    /// <summary>
    /// テナント情報を扱うコンテキスト
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        string? TenantId { get; }

        /// <summary>
        /// RLSのバイパス(=テナントを無視して全てのデータにアクセスする)を行うかどうか?
        /// </summary>
        /// <value></value>
        bool IsBypass { get; }

        /// <summary>
        /// 明示的にテナントIDを設定したスコープを作成する
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IDisposable CreateTenantIdScope(string tenantId);

        /// <summary>
        /// 明示的にRLSをバイパスするスコープを作成する
        /// </summary>
        /// <returns></returns>
        IDisposable CreateBypassScope();
    }
}