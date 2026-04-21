using Microsoft.Extensions.Hosting;

public class RemindersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    public RemindersBackgroundService(IServiceProvider provider) => _provider = provider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _provider.CreateScope())
            {
                var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();
                await reminderService.SendRemindersDueAsync();
            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // tous les 5 minutes
        }
    }
}