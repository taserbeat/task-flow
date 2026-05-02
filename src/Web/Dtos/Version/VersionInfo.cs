using System.Reflection;
using System.Text.Json.Serialization;

namespace Web.Dtos.Version
{
    /// <summary>
    /// バージョン情報
    /// </summary>
    public class VersionInfo
    {
        private static readonly string _version = LoadVersion();

        private static string LoadVersion()
        {
            // アセンブリのバージョン情報を取得
            var assembly = typeof(Program).Assembly;
            var assemblyVersionInfo = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return assemblyVersionInfo?.InformationalVersion ?? "undefined";
        }

        /// <summary>
        /// バージョン
        /// </summary>
        /// <value></value>
        [JsonPropertyName("version")]
        public string Version { get => _version; }
    }
}