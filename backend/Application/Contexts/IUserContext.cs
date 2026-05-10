using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;

namespace Application.Contexts
{
    /// <summary>
    /// ユーザー情報を扱うコンテキスト
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// 認証されているかどうか?
        /// </summary>
        /// <value></value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// テナントID
        /// </summary>
        /// <value></value>
        TenantId TenantId { get; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        /// <value></value>
        UserId UserId { get; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        UserEmail Email { get; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        RoleId RoleId { get; }

        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        RoleNameEnum RoleName { get; }

        /// <summary>
        /// ロールレベル
        /// </summary>
        /// <value></value>
        RoleLevelEnum RoleLevel { get; }
    }
}