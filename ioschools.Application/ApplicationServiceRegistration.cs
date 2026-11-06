using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ioschools.Application
{
    /// <summary>
    /// Application layer DI registration
    /// </summary>
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}
