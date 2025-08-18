// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class Worker(
        ILogger<Worker> log,
        IHostApplicationLifetime hostLifetime,
        NetCupDynDnsApiClient apiClient) : BackgroundService
    {
    #region Methods

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
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
                catch (Exception ex)
                {
                    log.LogError($"An error occurred while updating the DynDNS records. Exception Message: {ex.Message}");
                }
            }
        }

    #endregion
    }
}
