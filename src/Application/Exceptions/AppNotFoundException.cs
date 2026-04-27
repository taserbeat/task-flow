namespace Application.Exceptions
{
    /// <summary>
    /// リソースが見つからないエラーの例外
    /// </summary>
    public class AppNotFoundException : AppException
    {
        /// <summary>
        /// リソースが見つからないエラーのコンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public AppNotFoundException(string message) : base(message)
        {

        }
    }
}