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
        /// 表示名
        /// </summary>
        /// <value></value>
        public RoleLabel Label { get; private set; } = default!;

        /// <summary>
        /// ロールレベル (高いほど強い権限)
        /// </summary>
        /// <value></value>
        public RoleLevelEnum Level { get; private set; }

        public static RoleEm Create(RoleId roleId, RoleNameEnum name, RoleLabel label, RoleLevelEnum level)
        {
            return new RoleEm
            {
                Id = roleId,
                Name = name,
                Label = label,
                Level = level,
            };
        }

        /// <summary>
        /// ロールレベルが指定ロールよりも高いかチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsHigherLevelThan(RoleEm target)
        {
            return Level > target.Level;
        }

        /// <summary>
        /// ロールレベルが指定ロール以上であるかチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsHigherOrEqualLevelThan(RoleEm target)
        {
            return Level >= target.Level;
        }

        /// <summary>
        /// ロールレベルが指定ロールよりも低いかチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsLowerLevelThan(RoleEm target)
        {
            return Level < target.Level;
        }

        /// <summary>
        /// ロールレベルが指定ロール以下であるかチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsLowerOrEqualLevelThan(RoleEm target)
        {
            return Level <= target.Level;
        }

        /// <summary>
        /// ロールレベルが等しいかチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsEqualLevel(RoleEm target)
        {
            return Level == target.Level;
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