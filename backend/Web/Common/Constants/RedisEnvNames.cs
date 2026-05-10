namespace Web.Common.Constants
{
    /// <summary>
    /// Redisに関する環境変数名
    /// </summary>
    public class RedisEnvNames
    {
        /// <summary>
        /// Redisの接続文字列
        /// </summary>
        public const string RedisConnectionString = "RedisConnection";

        /// <summary>
        /// Redisの有効/無効を設定するキー
        /// </summary>
        public const string EnableRedis = "TF_ENABLE_REDIS";
    }
}