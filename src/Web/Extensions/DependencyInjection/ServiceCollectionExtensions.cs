using Application.Contexts;
using Infrastructure.Contexts;
using Web.Contexts;

namespace Web.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> の拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeb(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor>();
            services.AddScoped<IRlsContext, RlsContext>();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}