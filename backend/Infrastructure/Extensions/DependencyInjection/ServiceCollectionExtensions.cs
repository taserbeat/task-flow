using Application.Repositories;
using Infrastructure.Common.Constants;
using Infrastructure.DbContexts;
using Infrastructure.Interceptors;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> の拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// インフラストラクチャ層のサービスを登録する
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString(ConnectionStringNames.DefaultConnection));

                var isRlsEnabled = configuration[EnvNames.EnableRls]?.ToLower() == "true";
                if (isRlsEnabled)
                {
                    options.AddInterceptors([
                        provider.GetRequiredService<AppDbConnectionInterceptor>(),
                    ]);
                }
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}