namespace Application.Exceptions
{
    /// <summary>
    /// 認可エラーの例外
    /// </summary>
    public class AppForbiddenException : AppException
    {
        /// <summary>
        /// 認可エラーのコンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public AppForbiddenException(string message) : base(message)
        {

        }
    }
}