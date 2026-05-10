using Application.Contexts;
using Application.Services;
using Domain.Entities.Roles;

namespace Infrastructure.Services
{
    /// <summary>
    /// 認可サービス
    /// </summary>
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IUserContext _userContext;

        public AuthorizeService(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public bool HasRequiredRole(RoleLevelEnum requiredLevel)
        {
            if (!_userContext.IsAuthenticated)
            {
                return false;
            }

            return _userContext.RoleLevel >= requiredLevel;
        }
    }
}