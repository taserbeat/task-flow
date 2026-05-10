using Domain.Entities.Roles;
using Microsoft.AspNetCore.Authorization;

namespace Web.Authorization
{
    /// <summary>
    /// ユーザー定義のロールで必須権限の判定に使用するクラス
    /// </summary>
    public class RoleLevelRequirement : IAuthorizationRequirement
    {
        public RoleLevelEnum RequiredLevel { get; }

        public RoleLevelRequirement(RoleLevelEnum requiredLevel)
        {
            RequiredLevel = requiredLevel;
        }
    }
}