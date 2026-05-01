using System.Security.Claims;
using Application.Contexts;
using Application.Exceptions;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Web.Common.Constants;

namespace Web.Contexts
{
    /// <summary>
    /// ユーザー情報を扱うコンテキスト
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

        public TenantId TenantId
        {
            get
            {
                return new(Guid.Parse(GetClaim(CustomClaimTypes.TenantId)));
            }
        }

        public UserId UserId
        {
            get
            {
                return new(Guid.Parse(GetClaim(CustomClaimTypes.UserId)));
            }
        }

        public UserEmail Email
        {
            get
            {
                return new(GetClaim(CustomClaimTypes.Email));
            }
        }

        public RoleId RoleId
        {
            get
            {
                return new(Guid.Parse(GetClaim(CustomClaimTypes.RoleId)));
            }
        }

        public RoleNameEnum RoleName
        {
            get
            {
                var roleName = GetClaim(CustomClaimTypes.RoleName);
                return Enum.TryParse<RoleNameEnum>(roleName, out var roleCodeEnum) ? roleCodeEnum : throw new AppAuthenticateException("未認証エラーです。");
            }
        }

        private ClaimsPrincipal? _user => _contextAccessor.HttpContext?.User;

        private string GetClaim(string claimType)
        {
            return _user?.FindFirst(claimType)?.Value ?? throw new AppAuthenticateException("未認証エラーです。");
        }
    }
}