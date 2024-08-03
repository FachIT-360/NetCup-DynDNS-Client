namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class Worker(
        IHostApplicationLifetime hostLifetime,
        NetCupDynDnsApiClient apiClient) : BackgroundService
    {
    #region Methods

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait for completely application is started
                if (!hostLifetime.ApplicationStarted.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                    continue;
                }

                await apiClient.UpdateDynDns();

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

    #endregion
    }
}
