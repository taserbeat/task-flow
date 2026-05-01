using Domain.Entities.Common;

namespace Domain.Entities.Roles
{
    /// <summary>
    /// ロールのエンティティモデル
    /// </summary>
    public class RoleEm : BaseEm<RoleId>
    {
        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        public RoleNameEnum Name { get; private set; }

        public static RoleEm Create(RoleId roleId, RoleNameEnum name)
        {
            return new RoleEm
            {
                Id = roleId,
                Name = name,
            };
        }
    }

    /// <summary>
    /// ロールの名称
    /// </summary>
    public enum RoleNameEnum
    {
        /// <summary>
        /// ユーザー
        /// </summary>
        User = 0,

        /// <summary>
        /// 管理者
        /// </summary>
        Admin,

        /// <summary>
        /// システム管理者
        /// </summary>
        SystemAdmin,
    }
}