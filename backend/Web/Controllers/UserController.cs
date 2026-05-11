using Application.Contexts;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
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

        public UserController(ILogger<UserController> logger, IUserContext userContext, GetUsersUseCase getUsersUseCase)
        {
            _logger = logger;
            _userContext = userContext;
            _getUsersUseCase = getUsersUseCase;
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
                UserId = x.Id.Value,
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
        public ActionResult<CurrentUserResponse> GetCurrentUser()
        {
            return new CurrentUserResponse
            {
                TenantId = _userContext.TenantId.Value,
                UserId = _userContext.UserId.Value,
                Email = _userContext.Email.Value,
                RoleName = _userContext.RoleName,
                RoleLevel = (int)_userContext.RoleLevel
            };
        }
    }
}