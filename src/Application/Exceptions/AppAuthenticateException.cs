namespace Application.Exceptions
{
    /// <summary>
    /// 認証エラーの例外
    /// </summary>
    public class AppAuthenticateException : AppException
    {
        /// <summary>
        /// 認証エラーのコンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public AppAuthenticateException(string message) : base(message)
        {

        }
    }
}