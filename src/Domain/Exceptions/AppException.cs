namespace Domain.Exceptions
{
    /// <summary>
    /// アプリケーションの例外の基底クラス
    /// </summary>
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message)
        {

        }
    }
}
