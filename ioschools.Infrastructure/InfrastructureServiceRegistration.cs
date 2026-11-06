using ioschools.Application.Interfaces;
using ioschools.Domain.Interfaces;
using ioschools.Infrastructure.Data;
using ioschools.Infrastructure.Repositories;
using ioschools.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ioschools.Infrastructure
{
    /// <summary>
    /// Infrastructure layer DI registration
    /// Replaces legacy Global.asax Application_Start registrations
    /// </summary>
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // EF Core DbContext - replaces legacy LINQ-to-SQL DataContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("ioschools.Infrastructure")));

            // Repositories - replaces legacy Repository class
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IHomeworkRepository, HomeworkRepository>();
            services.AddScoped<ISchoolFeeRepository, SchoolFeeRepository>();
            services.AddScoped<IEmailQueueRepository, EmailQueueRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Infrastructure services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ISchedulerService, SchedulerService>();

            // Application services registered in Infrastructure (depend on EF Core)
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IHomeworkService, HomeworkService>();
            services.AddScoped<ISchoolFeeService, SchoolFeeService>();

            return services;
        }
    }
}
