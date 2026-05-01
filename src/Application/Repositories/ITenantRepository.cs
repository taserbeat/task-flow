using Domain.Entities.Tenants;

namespace Application.Repositories
{
    /// <summary>
    /// テナントのリポジトリ
    /// </summary>
    public interface ITenantRepository
    {
        /// <summary>
        /// テナントを追加する
        /// </summary>
        /// <param name="tenantEm"></param>
        /// <returns></returns>
        Task AddAsync(TenantEm tenantEm);

        /// <summary>
        /// 指定のテナントを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <returns></returns>
        Task<TenantEm?> GetByIdAsync(TenantId tenantId);
    }
}