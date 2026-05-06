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

        /// <summary>
        /// ロールレベル (高いほど強い権限)
        /// </summary>
        /// <value></value>
        public RoleLevelEnum Level { get; private set; }

        public static RoleEm Create(RoleId roleId, RoleNameEnum name, RoleLevelEnum level)
        {
            return new RoleEm
            {
                Id = roleId,
                Name = name,
                Level = level,
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

    /// <summary>
    /// ロールレベル (高いほど強い権限)
    /// </summary>
    public enum RoleLevelEnum
    {
        User = 10,
        Admin = 20,
        SystemAdmin = 30,
    }
}