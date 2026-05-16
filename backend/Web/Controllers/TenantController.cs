using Application.Contexts;
using Application.UseCases.Tenants;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.Tenants.CreateTenant;
using Web.Dtos.Tenants.GetTenant;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("テナント")]
    public class TenantController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly CreateTenantUseCase _createTenantUseCase;
        private readonly GetTenantsUseCase _getTenantUseCase;

        public TenantController(IUserContext userContext, CreateTenantUseCase createTenantUseCase, GetTenantsUseCase getTenantUseCase)
        {
            _userContext = userContext;
            _createTenantUseCase = createTenantUseCase;
            _getTenantUseCase = getTenantUseCase;
        }

        /// <summary>
        /// テナントの作成
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
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireSystemAdmin)]
        public async Task<IActionResult> CreateTenant(CreateTenantRequest request)
        {
            var param = new CreateTenantParam
            {
                Name = request.Name,
                InitUserParam = new CreateUserParam
                {
                    Email = request.InitUser.Email,
                    Password = request.InitUser.Password,
                    LastName = request.InitUser.LastName,
                    FirstName = request.InitUser.FirstName,
                    RoleId = request.InitUser.RoleId,
                }
            };

            await _createTenantUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, param);

            return Ok();
        }

        /// <summary>
        /// テナント一覧の取得
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireSystemAdmin)]
        public async Task<ActionResult<IEnumerable<TenantSummaryResponse>>> GetTenants()
        {
            var tenantEms = await _getTenantUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId);
            var response = tenantEms.Select(x => TenantSummaryResponse.FromEntity(x)).ToList();

            return response;
        }
    }
}