using Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Web.Authorization.Handlers
{
    /// <summary>
    /// ロールで必須権限を判定するハンドラ
    /// </summary>
    public class RoleLevelHandler : AuthorizationHandler<RoleLevelRequirement>
    {
        private readonly IAuthorizeService _authorizeService;

        public RoleLevelHandler(IAuthorizeService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleLevelRequirement requirement)
        {
            if (_authorizeService.HasRequiredRole(requirement.RequiredLevel))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}