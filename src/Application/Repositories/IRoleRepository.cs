using Domain.Entities.Roles;

namespace Application.Repositories
{
    /// <summary>
    /// ロールのリポジトリ
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// ロールを追加する
        /// </summary>
        /// <param name="roleEm"></param>
        /// <returns></returns>
        Task AddAsync(RoleEm roleEm);

        /// <summary>
        /// 指定のロールを取得する
        /// </summary>
        /// <param name="roleId">ロールID</param>
        /// <returns></returns>
        Task<RoleEm?> GetByIdAsync(RoleId roleId);

        /// <summary>
        /// 指定のロールを取得する
        /// </summary>
        /// <param name="roleName">ロール名</param>
        /// <returns></returns>
        Task<RoleEm?> GetByNameAsync(RoleNameEnum roleName);
    }
}