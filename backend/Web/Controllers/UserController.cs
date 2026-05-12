using Application.Contexts;
using Application.UseCases.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Users.CreateUser;
using Web.Dtos.Users.GetCurrentUser;
using Web.Dtos.Users.GetUser;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("ユーザー")]
    public class UserController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly GetUsersUseCase _getUsersUseCase;
        private readonly GetCurrentUserUseCase _getCurrentUserUseCase;
        private readonly CreateUserUseCase _createUserUseCase;

        public UserController(IUserContext userContext, GetUsersUseCase getUsersUseCase, GetCurrentUserUseCase getCurrentUserUseCase, CreateUserUseCase createUserUseCase)
        {
            _userContext = userContext;
            _getUsersUseCase = getUsersUseCase;
            _getCurrentUserUseCase = getCurrentUserUseCase;
            _createUserUseCase = createUserUseCase;
        }

        /// <summary>
        /// ユーザー一覧の取得
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
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
        /// ユーザーの作成
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var input = new CreateUserInput
            {
                TenantId = _userContext.TenantId,
                ActorId = _userContext.UserId,
                Email = request.Email,
                Password = request.Password,
                LastName = request.LastName,
                FirstName = request.FirstName,
                RoleId = request.RoleId,
            };

            await _createUserUseCase.Execute(input);

            return Ok();
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