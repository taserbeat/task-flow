using Application.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Dtos.Users.GetCurrentUser;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("ユーザー")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserContext _userContext;

        public UserController(ILogger<UserController> logger, IUserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
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