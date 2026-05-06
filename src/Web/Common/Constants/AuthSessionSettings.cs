namespace Web.Common.Constants
{
    /// <summary>
    /// 認証セッションの設定
    /// </summary>
    public static class AuthSessionSettings
    {
        /// <summary>
        /// 有効期限 (単位: 分)
        /// </summary>
        public const int ExpiredMinutes = 60;

        /// <summary>
        /// SlidingExpirationを有効にするか?
        /// </summary>
        public const bool AllowSlidingExpiration = true;
    }
}