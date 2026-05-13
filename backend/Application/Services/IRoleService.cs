using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Application.Services
{
    /// <summary>
    /// ロール権限サービス
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 実行者が対象ロールを所有する新規ユーザーを作成できるか判定する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="actorId">実行者</param>
        /// <param name="target">対象のロールID</param>
        /// <returns></returns>
        Task<bool> CanCreateUserAsync(TenantId tenantId, UserId actorId, RoleId target);

        /// <summary>
        /// 実行者が対象ユーザーを削除できるか判定する
        /// </summary>
        /// <param name="tenantId">テナントID</param>
        /// <param name="actorId">実行者</param>
        /// <param name="target">対象者</param>
        /// <returns></returns>
        Task<bool> CanDeleteUserAsync(TenantId tenantId, UserId actorId, UserId target);
    }
}