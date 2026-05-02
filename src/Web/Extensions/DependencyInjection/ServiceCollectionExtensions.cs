using Application.Contexts;
using Application.Repositories;
using Application.Services;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
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
            services.AddHttpContextAccessor();
            services.AddSingleton(TimeProvider.System);

            services.AddScoped<IRlsContext, RlsContext>();
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPasswordHashService, PasswordHashService>();

            return services;
        }
    }
}