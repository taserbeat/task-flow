using System.Reflection;
using Application.Extensions.DependencyInjection;
using Domain.Exceptions;
using Infrastructure.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using NLog.Extensions.Logging;
using Web.Attributes;
using Web.Common.Constants;
using Web.Controllers;
using Web.Dtos.Version;
using Web.Extensions.DependencyInjection;
using Web.Middlewares;
using Web.Workers;

var builder = WebApplication.CreateBuilder(args);

// ロガーの設定
builder.Logging
    .ClearProviders()
    .AddConsole()
    .SetMinimumLevel(LogLevel.Trace)
    .AddNLog();

// HostedServiceの登録
builder.Services.AddHostedService<InitWorker>();

// MVCの設定を追加
builder.Services.AddControllersWithViews();

// Cookieポリシーの設定
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = HttpOnlyPolicy.None;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// ルーティングの設定
builder.Services.Configure<RouteOptions>(options =>
{
    // ルーティングに小文字を許可する
    options.LowercaseUrls = true;
});

// 認証の設定
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/auth/login";

        // 未認証のリクエストに対するリダイレクト処理
        options.Events.OnRedirectToLogin = (context) =>
        {
            // APIへのリクエストの場合、ログインページへのリダイレクトは行わない (401エラーを返す)
            bool isApiRequest = context.Request.Path.StartsWithSegments("/api");
            if (isApiRequest)
            {
                throw new AppAuthenticateException("未認証エラー");
            }

            // ログインページにリダイレクトすることで、ユーザーにログインを促す
            string redirectPath = "/auth/login" + "?ReturnUrl=" + context.Request.PathBase + context.Request.Path;
            context.Response.Redirect(redirectPath);

            return Task.CompletedTask;
        };

        // アクセス権限が無いリクエストに対するリダイレクト処理をカスタマイズ
        options.Events.OnRedirectToAccessDenied = (context) =>
        {
            // 認可エラーの場合、403エラーを返す
            throw new AppForbiddenException("この操作を行う権限がありません。");
        };

        // Cookieの更新が必要であるかをチェックする処理
        options.Events.OnCheckSlidingExpiration = (context) =>
        {
            var endpoint = context.HttpContext.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<DisableSlidingExpirationAttribute>() != null)
            {
                // 指定のエンドポイントでは認証Cookieを更新しない
                context.ShouldRenew = false;
            }

            return Task.CompletedTask;
        };
    });

builder.Services
    .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) =>
    {
        // セッションストアに指定
        options.SessionStore = store;

        // 有効期限
        options.ExpireTimeSpan = TimeSpan.FromMinutes(AuthSessionSettings.ExpiredMinutes);

        // 有効期限のスライド(=有効期限が残り半分を過ぎていると自動で期限を更新)するか?
        options.SlidingExpiration = AuthSessionSettings.AllowSlidingExpiration;
    });

// セッション(一時データの格納用)の設定
builder.Services.AddSession(options =>
{
    // セッション情報を格納するクッキーはHttpOnly(JavaScriptでアクセス禁止)とする
    options.Cookie.HttpOnly = true;

    // クッキーのSameSite設定
    options.Cookie.SameSite = SameSiteMode.Lax;

    // 有効期限
    options.IdleTimeout = TimeSpan.FromMinutes(SessionSettings.ExpiredMinutes);

    // セッションデータと紐づけるCookieのキー名
    options.Cookie.Name = SessionSettings.CookieName;
});

// Swagger(Open API)の設定
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(SwaggerSettings.DocumentName, new OpenApiInfo()
    {
        Version = new VersionInfo().Version,
        Title = "TaskFlow API",
        Description = "TaskFlowのWeb APIです。",
    });

    // TODO: JWT認証を追加

    // TODO: Swaggerのカスタムフィルターを追加

    // summaryコメントでアノテーションする
    var xmlFile = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// DIコンテナへの登録
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 例外のハンドリングミドルウェアを追加
app.UseMiddleware<ExceptionHandlingMiddleware>();

// リバースプロキシ環境でクライアントのIPアドレスを記録できるように、Forwardedヘッダーの設定
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor,
});

// wwwrootフォルダの静的ファイルを公開
app.UseStaticFiles();

// Cookieポリシーのミドルウェアを使用
app.UseCookiePolicy();

// ルーティングのミドルウェアを使用
app.UseRouting();

// CORSのミドルウェアを使用
app.UseCors();

// 認証・認可のミドルウェアを使用
app.UseAuthentication();
app.UseAuthorization();

// セッションのミドルウェアを使用
app.UseSession();

// Swagger UIに認証をかける
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/swagger"),
    branch =>
    {
        branch.Use(async (context, next) =>
        {
            bool isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                await context.ChallengeAsync();
                return;
            }

            await next();
        });
    }
);

// Swaggerのミドルウェアを使用
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(SwaggerSettings.EndPointUrl, SwaggerSettings.DocumentName);
    options.DisplayRequestDuration();
});

// フロントエンドの静的コンテンツを返す対応
string frontendDir = app.Environment.IsProduction()
    ? FrontendController.StaticDirPathWithDevEnv
    : FrontendController.StaticDirPathWithDevEnv;
string frontendDirPath = Path.Combine(app.Environment.ContentRootPath, frontendDir);
var frontendFileProvider = new PhysicalFileProvider(frontendDirPath);
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = frontendFileProvider,
    OnPrepareResponse = (ctx) =>
    {
        bool isAuthenticated = ctx.Context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            ctx.Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Body = Stream.Null;
        }

        // フロントエンドのコンテンツはキャッシュを無効
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
    },
});

// コントローラーのマッピング
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.MapFallbackToController(nameof(FrontendController.Index), FrontendController.ControllerName);

// 開発環境の場合、カレントディレクトリを実行ファイル(exe)の場所に設定する
if (app.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.BaseDirectory))
{
    Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
}

app.Run();
