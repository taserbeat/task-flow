namespace Infrastructure.Common.Constants
{
    /// <summary>
    /// 接続文字列の名前
    /// </summary>
    public static class ConnectionStringNames
    {
        /// <summary>
        /// デフォルトの接続文字列
        /// </summary>
        public const string DefaultConnection = "DefaultConnection";

        /// <summary>
        /// マイグレーション用の接続文字列
        /// </summary>
        public const string MigrateConnection = "MigrateConnection";
    }
}