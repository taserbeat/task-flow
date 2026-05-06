using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Dtos.Version;

namespace Web.Models
{
    /// <summary>
    /// ログイン画面のViewModel
    /// </summary>
    public class LoginViewModel : PageModel
    {
        /// <summary>
        /// アプリのバージョン
        /// </summary>
        /// <value></value>
        public string Version { get; private set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        /// <value></value>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// 認証後のリダイレクト先URL
        /// </summary>
        /// <value></value>
        public string? ReturnUrl { get; private set; }

        public LoginViewModel(string? errorMessage = null, string? returnUrl = null)
        {
            Version = new VersionInfo().Version;
            ErrorMessage = errorMessage;
            ReturnUrl = returnUrl;
        }

        public LoginViewModel SetErrorMessage(string? errorMessage = null)
        {
            ErrorMessage = errorMessage;

            return this;
        }

        public LoginViewModel SetReturnUrl(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            return this;
        }
    }
}