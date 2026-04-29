using Application.Repositories;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// トランザクションを管理する
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction? _transaction = null;
        private readonly TaskFlowDbContext _dbContext;

        public UnitOfWork(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public bool IsInTransaction => _transaction != null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            // 既にトランザクションが開始されている場合は何もしない
            if (_transaction != null)
            {
                return;
            }

            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await BeginTransactionAsync(cancellationToken);

            try
            {
                await action();
                await CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}