namespace Web.Common.Constants
{
    /// <summary>
    /// セッション(一時データ)の設定
    /// </summary>
    public static class SessionSettings
    {
        /// <summary>
        /// 有効期限 (単位: 分)
        /// </summary>
        public const int ExpiredMinutes = 10;

        /// <summary>
        /// ユーザーのセッションデータを格納するCookieのキー名
        /// </summary>
        public const string CookieName = ".AspNetCore.Session";
    }
}