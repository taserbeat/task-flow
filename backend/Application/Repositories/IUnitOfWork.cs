namespace Application.Repositories
{
    /// <summary>
    /// トランザクションとRLSの境界を管理するインターフェース
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 更新クエリを実行する
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションが開始されているかどうか
        /// </summary>
        /// <value>トランザクションを開始している場合はtrue、開始していなければfalse</value>
        bool IsInTransaction { get; }

        /// <summary>
        /// トランザクションを開始する
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションをコミットする
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションをロールバックする
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 指定のデリゲートをトランザクションで実行する
        /// </summary>
        /// <param name="action">実行するデリゲート</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

        /// <summary>
        /// 明示的にテナントIDを設定したスコープを作成する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <returns></returns>
        IDisposable CreateTenantIdScope(string tenantId);

        /// <summary>
        /// 明示的にRLSをバイパスするスコープを作成する
        /// </summary>
        /// <returns></returns>
        IDisposable CreateBypassScope();
    }
}