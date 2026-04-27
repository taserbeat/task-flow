namespace Application.Exceptions
{
    /// <summary>
    /// バリデーションエラーの例外
    /// </summary>
    public class AppValidateException : AppException
    {
        /// <summary>
        /// バリデーションエラーのコンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public AppValidateException(string message) : base(message)
        {

        }
    }
}