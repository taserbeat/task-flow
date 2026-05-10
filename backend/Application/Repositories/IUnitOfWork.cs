namespace Application.Repositories
{
    /// <summary>
    /// トランザクションを管理するインターフェース
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
    }
}