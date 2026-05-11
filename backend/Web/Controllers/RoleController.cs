using Application.UseCases.Roles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Dtos.Roles;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Tags("ロール")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly GetRolesUseCase _getRolesUseCase;

        public RoleController(ILogger<RoleController> logger, GetRolesUseCase getRolesUseCase)
        {
            _logger = logger;
            _getRolesUseCase = getRolesUseCase;
        }

        /// <summary>
        /// ロール一覧の取得
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<RoleDetailResponse>>> GetRoles()
        {
            var roleEms = await _getRolesUseCase.Execute();
            var response = roleEms
                .Select(x => RoleDetailResponse.FromEntity(x))
                .ToList();

            return response;
        }
    }
}