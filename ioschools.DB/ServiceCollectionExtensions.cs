using ioschools.DB.Application.Interfaces;
using ioschools.DB.Infrastructure.Data;
using ioschools.DB.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ioschools.DB
{
    /// <summary>
    /// Extension methods for registering ioschools.DB services with the
    /// ASP.NET Core dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="IoschoolsDbContext"/> and <see cref="IRepository"/>
        /// with the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddIoschoolsDb(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<IoschoolsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IRepository, Repository>();

            return services;
        }
    }
}
