namespace Web.Common.Constants
{
    /// <summary>
    /// 認可のポリシー名
    /// </summary>
    public static class AuthorizePolicyNames
    {
        /// <summary>
        /// ユーザー権限が必須のポリシー
        /// </summary>
        public const string RequireUser = "RequireUser";

        /// <summary>
        /// 管理者権限が必須のポリシー
        /// </summary>
        public const string RequireAdmin = "RequireAdmin";

        /// <summary>
        /// システム管理者権限が必須のポリシー
        /// </summary>
        public const string RequireSystemAdmin = "RequireSystemAdmin";
    }
}