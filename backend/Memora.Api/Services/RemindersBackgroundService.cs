using Memora.Api.Services;

public class RemindersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<RemindersBackgroundService> _logger;

    public RemindersBackgroundService(
        IServiceProvider provider,
        ILogger<RemindersBackgroundService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();

                await reminderService.SendRemindersDueAsync();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Arrêt normal de l'application.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reminder background service failed but the app will continue running.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}