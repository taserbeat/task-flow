using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using Xunit;

namespace Web.IntegrationTests.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// HTTPレスポンスが Success(200番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsOk(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.OK, self.StatusCode);
        }

        /// <summary>
        /// HTTPレスポンスが Success(200番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="content">文字列のコンテンツ</param>
        public static void IsOk(this HttpResponseMessage self, string content)
        {
            Assert.Equal(HttpStatusCode.OK, self.StatusCode);

            var task = self.Content.ReadAsStringAsync();
            task.Wait();

            Assert.Equal(content, task.Result);
        }

        /// <summary>
        /// HTTPレスポンスが Success(200番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="content"></param>
        /// <typeparam name="T"></typeparam>
        public static void IsOk<T>(this HttpResponseMessage self, T content)
        {
            var jsonString = JsonSerializer.Serialize(content, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });

            self.IsOk(jsonString);
        }

        /// <summary>
        /// HTTPレスポンスが Redirect(302番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsRedirect(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.Redirect, self.StatusCode);
        }

        /// <summary>
        /// HTTPレスポンスが BadRequest(400番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsBadRequest(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.BadRequest, self.StatusCode);
        }

        /// <summary>
        /// HTTP レスポンスが Unauthorized(401番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsUnauthorized(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, self.StatusCode);
        }

        /// <summary>
        /// HTTP レスポンスが Forbidden(403番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsForbidden(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.Forbidden, self.StatusCode);
        }

        /// <summary>
        /// HTTP レスポンスが NotFound(404番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsNotFound(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.NotFound, self.StatusCode);
        }

        /// <summary>
        /// HTTP レスポンスが InternalServerError(500番) であることを検証する
        /// </summary>
        /// <param name="self"></param>
        public static void IsInternalServerError(this HttpResponseMessage self)
        {
            Assert.Equal(HttpStatusCode.InternalServerError, self.StatusCode);
        }
    }
}