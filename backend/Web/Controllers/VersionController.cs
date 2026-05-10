using Microsoft.AspNetCore.Mvc;
using Web.Dtos.Version;

namespace Web.Controllers
{
    /// <summary>
    /// バージョン情報のコントローラー
    /// </summary>
    [ApiController]
    [Route("/api/version")]
    [Tags("バージョン")]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// バージョン情報の取得
        /// </summary>
        /// <returns></returns>
        /// <response code="200">OK</response>
        /// <response code="400">リクエストが不正</response>
        /// <response code="401">未認証エラー</response>
        /// <response code="404">存在しないURL</response>
        /// <response code="500">サーバーが処理に失敗</response>
        [HttpGet]
        [Route("")]
        public ActionResult<VersionInfo> GetVersion()
        {
            return new VersionInfo();
        }
    }
}