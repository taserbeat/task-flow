using Domain.Entities.Tenants;

namespace Application.Repositories
{
    /// <summary>
    /// テナントリポジトリ
    /// </summary>
    public interface ITenantRepository
    {
        /// <summary>
        /// テナントを追加する
        /// </summary>
        /// <param name="tenantEm"></param>
        /// <returns></returns>
        Task AddAsync(TenantEm tenantEm);
    }
}