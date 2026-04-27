using System.Security.Claims;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares
{
    /// <summary>
    /// 例外をハンドリングするミドルウェア
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// ミドルウェアの処理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                await HandleAppExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleUnknownException(context, ex);
            }
        }

        /// <summary>
        /// <see cref="AppException"/>を処理する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex switch
            {
                AppValidateException => StatusCodes.Status400BadRequest,
                AppAuthenticateException => StatusCodes.Status401Unauthorized,
                AppForbiddenException => StatusCodes.Status403Forbidden,
                AppNotFoundException => StatusCodes.Status404NotFound,

                // その他の例外はBad Requestとする
                _ => StatusCodes.Status400BadRequest
            };

            var response = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = ex.Message,
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        /// <summary>
        /// 未知の例外を処理する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task HandleUnknownException(HttpContext context, Exception ex)
        {
            var logInfo = new
            {
                TraceId = context.TraceIdentifier,
                ex.Message,
                context.Request.Method,
                Path = context.Request.Path.ToString(),
                Query = context.Request.QueryString.ToString(),
                User = context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            };

            _logger.LogError(ex, "予期しないエラーが発生しました。 {@LogInfo}", logInfo);

            context.Response.StatusCode = 500;

            var response = new ProblemDetails
            {
                Title = "予期しないエラーが発生しました。",
                Status = context.Response.StatusCode,
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}