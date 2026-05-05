using Application.Contexts;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Web.Controllers
{
    /// <summary>
    /// フロントエンドのWebアプリにフォールバックするコントローラー
    /// </summary>
    public class FrontendController : Controller
    {
        private readonly ILogger<FrontendController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserContext _userContext;

        /// <summary>
        /// コントローラー名
        /// </summary>
        /// <value></value>
        public static string ControllerName
        {
            get => nameof(FrontendController).Replace("Controller", "");
        }

        /// <summary>
        /// 開発環境での静的コンテンツのディレクトリパス
        /// </summary>
        public static string StaticDirPathWithDevEnv = "privateroot";

        /// <summary>
        /// 本番環境での静的コンテンツのディレクトリパス
        /// </summary>
        public static string StaticDirPathWithProdEnv = "../../frontend/dist";

        public FrontendController(ILogger<FrontendController> logger, IWebHostEnvironment environment, IUserContext userContext)
        {
            _logger = logger;
            _environment = environment;
            _userContext = userContext;
        }

        /// <summary>
        /// フロントエンドのWebアプリを返す
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            bool isApiRequest = Request.Path.StartsWithSegments("/api");
            if (isApiRequest)
            {
                // 存在しないAPIへのアクセスなので、404エラーを返す
                throw new AppNotFoundException("指定されたURLは存在しません。");
            }

            if (!_userContext.IsAuthenticated)
            {
                // 未認証ユーザーによるアクセスなので、ログイン画面にリダイレクトする
                string returnUrl = Request.Path + Request.QueryString;

                // ルート直下へのアクセスはreturnUrlを付けない
                if (returnUrl == "/")
                {
                    return RedirectToAction(nameof(AuthController.Index), AuthController.ControllerName);
                }

                return RedirectToAction(nameof(AuthController.Index), AuthController.ControllerName, new { returnUrl });
            }

            // 認証済みの場合はフロントエンドのWebアプリを返す
            // string frontendDirPath = _environment.IsProduction() ? StaticDirPathWithProdEnv : StaticDirPathWithDevEnv;
            string frontendDirPath = _environment.IsProduction() ? StaticDirPathWithDevEnv : StaticDirPathWithDevEnv;
            var fileProvider = new PhysicalFileProvider(Path.Combine(_environment.ContentRootPath, frontendDirPath));
            var htmlFileInfo = fileProvider.GetFileInfo("index.html");

            if (!htmlFileInfo.Exists || string.IsNullOrWhiteSpace(htmlFileInfo.PhysicalPath))
            {
                return NotFound("フロントエンドのアプリが見つかりません。");
            }

            // フロントエンドのコンテンツはキャッシュを無効
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Pragma", "no-cache");

            return PhysicalFile(htmlFileInfo.PhysicalPath, "text/html");
        }
    }
}