using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Testing;
using Web.IntegrationTests.Factories;

namespace Web.IntegrationTests.Extensions
{
    /// <summary>
    /// <see cref="CustomWebApplicationFactory"/> の拡張メソッド
    /// </summary>
    public static class CustomWebApplicationFactoryExtensions
    {
        public static readonly WebApplicationFactoryClientOptions DefaultClientOptions = new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        };

        /// <summary>
        /// 認証なしでHTTPクライアントを作成し、指定したアクションを実行する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task RunWithoutAuthenticationAsync(this CustomWebApplicationFactory self, Func<HttpClient, Task> action)
        {
            self.AuthContext.WithUnauthenticated();
            self.AuthContext.WithEmptyClaims();

            using var client = self.CreateClient(DefaultClientOptions);

            await action(client);
        }

        /// <summary>
        /// 認証情報を設定してHTTPクライアントを作成し、指定したアクションを実行する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="claims"></param>
        /// <param name="action"></param>
        /// <returns></returns> <summary>
        public static async Task RunWithAuthenticationAsync(this CustomWebApplicationFactory self, IEnumerable<Claim> claims, Func<HttpClient, Task> action)
        {
            // 認証が通るようにモックする
            self.AuthContext.WithAuthenticated();
            self.AuthContext.Claims = claims.ToList();

            using var client = self.CreateClient(DefaultClientOptions);

            await action(client);
        }
    }
}