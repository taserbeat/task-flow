using System.Security.Claims;
using Domain.Entities.Roles;
using Web.Common.Constants;

namespace Web.IntegrationTests.Contexts
{
    public class TestAuthContext
    {
        public bool IsAuthenticated { get; set; }

        public List<Claim> Claims { get; set; } = [];

        public TestAuthContext WithAuthenticated()
        {
            IsAuthenticated = true;
            return this;
        }

        public TestAuthContext WithUnauthenticated()
        {
            IsAuthenticated = false;
            return this;
        }

        public TestAuthContext WithTenantIdClaim(string tenantId)
        {
            Claims.Add(new Claim(CustomClaimTypes.TenantId, tenantId));
            return this;
        }

        public TestAuthContext WithUserIdClaim(string userId)
        {
            Claims.Add(new Claim(CustomClaimTypes.UserId, userId));
            return this;
        }

        public TestAuthContext WithEmailClaim(string email)
        {
            Claims.Add(new Claim(CustomClaimTypes.Email, email));
            return this;
        }

        public TestAuthContext WithRoleIdClaim(string roleId)
        {
            Claims.Add(new Claim(CustomClaimTypes.RoleId, roleId));
            return this;
        }

        public TestAuthContext WithRoleNameClaim(RoleNameEnum roleNameEnum)
        {
            Claims.Add(new Claim(CustomClaimTypes.RoleName, roleNameEnum.ToString()));
            return this;
        }

        public TestAuthContext WithRoleLevelClaim(RoleLevelEnum roleLevelEnum)
        {
            Claims.Add(new Claim(CustomClaimTypes.RoleLevel, ((int)roleLevelEnum).ToString()));
            return this;
        }

        public TestAuthContext WithSessionIdClaim(string sessionId)
        {
            Claims.Add(new Claim(CustomClaimTypes.SessionId, sessionId));
            return this;
        }

        public TestAuthContext WithEmptyClaims()
        {
            Claims = new();
            return this;
        }
    }
}