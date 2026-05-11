using Application.Contexts;
using Application.UseCases.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Users.GetCurrentUser;
using Web.Dtos.Users.GetUser;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("ユーザー")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserContext _userContext;
        private readonly GetUsersUseCase _getUsersUseCase;
        private readonly GetCurrentUserUseCase _getCurrentUserUseCase;

        public UserController(ILogger<UserController> logger, IUserContext userContext, GetUsersUseCase getUsersUseCase, GetCurrentUserUseCase getCurrentUserUseCase)
        {
            _logger = logger;
            _userContext = userContext;
            _getUsersUseCase = getUsersUseCase;
            _getCurrentUserUseCase = getCurrentUserUseCase;
        }

        /// <summary>
        /// ユーザー一覧の取得
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<ActionResult<IEnumerable<UserSummaryResponse>>> GetUsers()
        {
            var userEms = await _getUsersUseCase.Execute(_userContext.TenantId);
            var users = userEms.Select(x => new UserSummaryResponse
            {
                Id = x.Id.Value,
                Email = x.Email.Value,
                Username = x.Username.FullName,
                RoleName = x.Role.Name,
            }).ToList();

            return users;
        }

        /// <summary>
        /// 自身のユーザー情報を取得する
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("me")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser()
        {
            var (tenantEm, userEm) = await _getCurrentUserUseCase.Execute(_userContext.TenantId, _userContext.UserId);
            if (tenantEm is null || userEm is null)
            {
                throw new AppAuthenticateException("未認証エラー");
            }

            return new CurrentUserResponse
            {
                Tenant = TenantDetailResponse.FromEntity(tenantEm),
                User = UserDetailResponse.FromEntity(userEm),
            };
        }
    }
}