using Application.UseCases.Auth;
using Application.UseCases.Roles;
using Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// アプリケーション層に必要なサービスを登録する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<GetRolesUseCase>();

            services.AddScoped<LoginUseCase>();
            services.AddScoped<GetCurrentUserUseCase>();
            services.AddScoped<GetUserUseCase>();
            services.AddScoped<GetUsersUseCase>();
            services.AddScoped<CreateUserUseCase>();
            services.AddScoped<UpdateUserUseCase>();
            services.AddScoped<DeleteUserUseCase>();

            return services;
        }
    }
}