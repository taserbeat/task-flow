using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Application.Repositories
{
    /// <summary>
    /// ユーザーのリポジトリ
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// ユーザーを追加する
        /// </summary>
        /// <param name="userEm"></param>
        /// <returns></returns>
        Task AddAsync(UserEm userEm);

        /// <summary>
        /// ユーザー一覧を取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <returns></returns>
        Task<IEnumerable<UserEm>> GetUsersAsync(TenantId tenantId);

        /// <summary>
        /// 指定のユーザーを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="isIncludeRole">ロールを含めるか?</param>
        /// <returns></returns>
        Task<UserEm?> GetByIdAsync(TenantId tenantId, UserId userId, bool isIncludeRole = false);

        /// <summary>
        /// ログイン処理として、メールアドレスからユーザーを取得する
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <returns></returns>
        Task<UserEm?> GetForLoginAsync(UserEmail email);

        /// <summary>
        /// 指定ユーザーのロールを取得する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns></returns>
        Task<RoleEm?> GetRoleByUserIdAsync(TenantId tenantId, UserId userId);

        /// <summary>
        /// 指定ユーザーを削除する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除したレコード数</returns>
        Task<int> DeleteAsync(TenantId tenantId, UserId userId);
    }
}