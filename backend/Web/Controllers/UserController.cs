using Application.Contexts;
using Application.UseCases.Users;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Users.CreateUser;
using Web.Dtos.Users.GetCurrentUser;
using Web.Dtos.Users.GetUser;
using Web.Dtos.Users.UpdateUser;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Tags("ユーザー")]
    public class UserController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly CreateUserUseCase _createUserUseCase;
        private readonly GetUsersUseCase _getUsersUseCase;
        private readonly GetUserUseCase _getUserUseCase;
        private readonly GetCurrentUserUseCase _getCurrentUserUseCase;
        private readonly UpdateUserUseCase _updateUserUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;

        public UserController(IUserContext userContext, CreateUserUseCase createUserUseCase, GetUsersUseCase getUsersUseCase, GetUserUseCase getUserUseCase, GetCurrentUserUseCase getCurrentUserUseCase, UpdateUserUseCase updateUserUseCase, DeleteUserUseCase deleteUserUseCase)
        {
            _userContext = userContext;
            _createUserUseCase = createUserUseCase;
            _getUsersUseCase = getUsersUseCase;
            _getUserUseCase = getUserUseCase;
            _getCurrentUserUseCase = getCurrentUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
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
            var param = new CreateUserParam
            {
                Email = request.Email,
                Password = request.Password,
                LastName = request.LastName,
                FirstName = request.FirstName,
                RoleId = request.RoleId,
            };

            await _createUserUseCase.Execute(_userContext.TenantId, _userContext.UserId, param);

            return Ok();
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
            var users = userEms.Select(x => UserSummaryResponse.FromEntity(x)).ToList();

            return users;
        }

        /// <summary>
        /// ユーザーの取得
        /// </summary>
        /// <param name="userId">取得対象のユーザーID</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="404">存在しない</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("{userId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<ActionResult<UserDetailResponse>> GetUser([FromRoute] Guid userId)
        {
            var userEm = await _getUserUseCase.Execute(_userContext.TenantId, UserId.New(userId));
            if (userEm is null)
            {
                return NotFound();
            }

            return UserDetailResponse.FromEntity(userEm);
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

        /// <summary>
        /// ユーザーの更新
        /// </summary>
        /// <param name="userId">更新対象のユーザーID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPut]
        [Route("{userId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, UpdateUserRequest request)
        {
            var param = new UpdateUserParam
            {
                Email = request.Email,
                Password = request.Password,
                LastName = request.LastName,
                FirstName = request.FirstName,
                RoleId = request.RoleId,
                IsActive = request.IsActive,
            };

            await _updateUserUseCase.Execute(_userContext.TenantId, _userContext.UserId, UserId.New(userId), param);

            return Ok();
        }

        /// <summary>
        /// ユーザーの削除
        /// </summary>
        /// <param name="userId">削除対象のユーザーID</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpDelete]
        [Route("{userId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireAdmin)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            await _deleteUserUseCase.Execute(_userContext.TenantId, _userContext.UserId, UserId.New(userId));

            return Ok();
        }
    }
}