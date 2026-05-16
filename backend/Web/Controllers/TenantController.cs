using Application.Contexts;
using Application.UseCases.Tenants;
using Application.UseCases.Users;
using Domain.Entities.Tenants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Dtos.Tenants.CreateTenant;
using Web.Dtos.Tenants.GetTenant;
using Web.Dtos.Tenants.UpdateTenant;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("テナント")]
    public class TenantController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly CreateTenantUseCase _createTenantUseCase;
        private readonly GetTenantsUseCase _getTenantsUseCase;
        private readonly GetTenantUseCase _getTenantUseCase;
        private readonly UpdateTenantUseCase _updateTenantUseCase;
        private readonly DeleteTenantUseCase _deleteTenantUseCase;

        public TenantController(IUserContext userContext, CreateTenantUseCase createTenantUseCase, GetTenantsUseCase getTenantsUseCase, GetTenantUseCase getTenantUseCase, UpdateTenantUseCase updateTenantUseCase, DeleteTenantUseCase deleteTenantUseCase)
        {
            _userContext = userContext;
            _createTenantUseCase = createTenantUseCase;
            _getTenantsUseCase = getTenantsUseCase;
            _getTenantUseCase = getTenantUseCase;
            _updateTenantUseCase = updateTenantUseCase;
            _deleteTenantUseCase = deleteTenantUseCase;
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
            var tenantEms = await _getTenantsUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId);
            var response = tenantEms.Select(x => TenantSummaryResponse.FromEntity(x)).ToList();

            return response;
        }

        /// <summary>
        /// テナントの取得
        /// </summary>
        /// <param name="tenantId">取得対象のテナントID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tenantId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireSystemAdmin)]
        public async Task<ActionResult<TenantDetailResponse>> GetTenant([FromRoute] Guid tenantId)
        {
            var tenantEm = await _getTenantUseCase.ExecuteAsync(TenantId.New(tenantId));
            if (tenantEm is null)
            {
                return NotFound();
            }

            return TenantDetailResponse.FromEntity(tenantEm);
        }

        /// <summary>
        /// テナントの更新
        /// </summary>
        /// <param name="tenantId">更新対象のテナントID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpPut]
        [Route("{tenantId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireSystemAdmin)]
        public async Task<IActionResult> UpdateTenant([FromRoute] Guid tenantId, UpdateTenantRequest request)
        {
            var param = new UpdateTenantParam
            {
                Name = request.Name,
            };

            await _updateTenantUseCase.ExecuteAsync(_userContext.UserId, TenantId.New(tenantId), param);

            return Ok();
        }

        /// <summary>
        /// テナントの削除
        /// </summary>
        /// <param name="tenantId">削除対象のテナントID</param>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="403">権限エラー</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpDelete]
        [Route("{tenantId}")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizePolicyNames.RequireSystemAdmin)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid tenantId)
        {
            await _deleteTenantUseCase.ExecuteAsync(_userContext.TenantId, _userContext.UserId, TenantId.New(tenantId));

            return Ok();
        }
    }
}