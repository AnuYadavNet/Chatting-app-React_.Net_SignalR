using ChattingApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChattingApp.Application.BackgroundServices
{
    /// <summary>
    /// IHostedService-based background worker that periodically cleans up
    /// messages older than 12 hours.
    ///
    /// WHY IHostedService OVER SQL JOB?
    /// - No dependency on SQL Server Agent (not always available in cloud environments)
    /// - Runs inside the application process — easier to deploy, monitor, and test
    /// - Fully controlled by .NET DI and lifecycle management
    /// - Can be unit-tested and easily replaced (e.g., swap interval without DBA involvement)
    ///
    /// The trade-off: if the app is down, the job won't run.
    /// For production critical cleanup, complement with a SQL Job as a safety net.
    /// </summary>
    public class MessageCleanupService : BackgroundService
    {
        // Runs every hour; messages are flagged after 12h by the SP
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MessageCleanupService> _logger;

        public MessageCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<MessageCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MessageCleanupService started. Interval: {Interval}", CleanupInterval);

            // Delay first run by 1 minute to let the app fully start
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Use a scope because IChatService is Scoped, not Singleton
                    using var scope = _scopeFactory.CreateScope();
                    var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                    await chatService.CleanupOldMessagesAsync();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // Log and continue — a cleanup failure should not crash the app
                    _logger.LogError(ex, "Error during message cleanup cycle.");
                }

                await Task.Delay(CleanupInterval, stoppingToken);
            }

            _logger.LogInformation("MessageCleanupService stopped.");
        }
    }
}
