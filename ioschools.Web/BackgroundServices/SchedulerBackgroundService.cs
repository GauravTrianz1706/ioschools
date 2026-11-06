using ioschools.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ioschools.Web.BackgroundServices
{
    /// <summary>
    /// Background scheduler service - replaces legacy CacheScheduler
    /// 
    /// Legacy approach: HttpRuntime.Cache with CacheItemRemovedCallback
    /// - Used System.Web.Caching.Cache
    /// - Triggered by cache expiry callbacks
    /// - Required HTTP ping to re-register cache entries
    /// 
    /// New approach: IHostedService / BackgroundService
    /// - Uses .NET 8 hosted services
    /// - Runs on a timer using PeriodicTimer
    /// - No HTTP ping required
    /// - Proper cancellation token support
    /// </summary>
    public class SchedulerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SchedulerBackgroundService> _logger;

        // Replaces CacheTimerType enum intervals
        private static readonly TimeSpan EmailInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan StudentDeactivationInterval = TimeSpan.FromHours(1);

        private DateTime _lastEmailRun = DateTime.MinValue;
        private DateTime _lastStudentDeactivationRun = DateTime.MinValue;

        public SchedulerBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<SchedulerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler background service started");

            // Use PeriodicTimer for efficient async waiting
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await RunScheduledTasksAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error in scheduler background service");
                }
            }
        }

        private async Task RunScheduledTasksAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Run email processing every 5 minutes
            if (now - _lastEmailRun >= EmailInterval)
            {
                await RunTaskAsync("SendPendingEmails", async (scheduler) =>
                    await scheduler.SendPendingEmailsAsync());
                _lastEmailRun = now;
            }

            // Run student deactivation check every hour
            if (now - _lastStudentDeactivationRun >= StudentDeactivationInterval)
            {
                await RunTaskAsync("SetStudentInactive", async (scheduler) =>
                    await scheduler.SetStudentInactiveIfLeavingDateSetAsync());
                _lastStudentDeactivationRun = now;
            }
        }

        private async Task RunTaskAsync(string taskName, Func<ISchedulerService, Task> task)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scheduler = scope.ServiceProvider.GetRequiredService<ISchedulerService>();
                _logger.LogDebug("Running scheduled task: {TaskName}", taskName);
                await task(scheduler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled task {TaskName} failed", taskName);
            }
        }
    }
}
