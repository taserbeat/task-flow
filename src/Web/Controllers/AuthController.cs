using System.Security.Claims;
using Application.Contexts;
using Application.UseCases.Auth;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Web.Common.Constants;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// 認証処理のコントローラー
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserContext _userContext;
        private readonly LoginUseCase _loginUseCase;

        /// <summary>
        /// コントローラー名
        /// </summary>
        /// <value></value>
        public static string ControllerName
        {
            get => nameof(AuthController).Replace("Controller", "");
        }

        public AuthController(ILogger<AuthController> logger, IUserContext userContext, LoginUseCase loginUseCase)
        {
            _logger = logger;
            _userContext = userContext;
            _loginUseCase = loginUseCase;
        }

        /// <summary>
        /// ログインページ
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("login")]
        public IActionResult Index(string? returnUrl = null)
        {
            if (_userContext.IsAuthenticated)
            {
                return RedirectToFrontend(returnUrl);
            }

            var viewModel = new LoginViewModel().SetReturnUrl(returnUrl);

            return View(viewModel);
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string? email, string? password, string? returnUrl = null)
        {
            var viewModel = new LoginViewModel().SetReturnUrl(returnUrl);

            if (string.IsNullOrWhiteSpace(email))
            {
                viewModel.SetErrorMessage("メールアドレスを入力してください。");
                return View(nameof(Index), viewModel);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                viewModel.SetErrorMessage("パスワードを入力してください。");
                return View(nameof(Index), viewModel);
            }

            var request = new LoginRequest(new UserEmail(email), password);

            LoginResult loginResult;
            try
            {
                loginResult = await _loginUseCase.ExecuteAsync(request);
            }
            catch (AppValidateException ex)
            {
                viewModel.SetErrorMessage(ex.Message);
                return View(nameof(Index), viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Cookie認証に失敗しました。 {Message}", ex.Message);

                viewModel.SetErrorMessage("予期しないエラーが発生しました。");
                return View(nameof(Index), viewModel);
            }

            // 認証クッキー発行
            var claims = new[]
            {
                new Claim(CustomClaimTypes.TenantId, loginResult.TenantId.ToString()),
                new Claim(CustomClaimTypes.UserId, loginResult.UserId.ToString()),
                new Claim(CustomClaimTypes.Email, loginResult.Email.ToString()),
                new Claim(CustomClaimTypes.RoleId, loginResult.RoleId.ToString()),
                new Claim(CustomClaimTypes.RoleName, loginResult.RoleName.ToString()),
                new Claim(CustomClaimTypes.RoleLevel, loginResult.RoleLevel.ToString()),
                new Claim(CustomClaimTypes.SessionId, loginResult.SessionId.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties()
            {
                // 認証セッションを持続させる
                IsPersistent = true,

                // 有効期限
                ExpiresUtc = DateTime.UtcNow.AddMinutes(AuthSessionSettings.ExpiredMinutes),

                // 有効期限のスライド
                // (サーバーへのアクセス時に有効期限が残り半分を過ぎていると期限を自動更新するか?)
                AllowRefresh = AuthSessionSettings.AllowSlidingExpiration,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToFrontend(returnUrl);
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// フロントエンドのWebアプリにリダイレクトする
        /// </summary>
        /// <param name="returnUrl">リダイレクト先URL</param>
        /// <returns></returns>
        private IActionResult RedirectToFrontend(string? returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(FrontendController.Index), FrontendController.ControllerName);
        }
    }
}