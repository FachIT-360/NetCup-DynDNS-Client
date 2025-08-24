// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.AppSettings;

using Microsoft.Extensions.Options;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class WorkerService : BackgroundService
    {
    #region Constants - Static fields - Fields

        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly ILogger<WorkerService>   _log;
        private readonly IServiceProvider         _serviceProvider;

    #endregion

    #region Constructors and Destructors

        public WorkerService(ILogger<WorkerService> log,
                             IHostApplicationLifetime hostLifetime,
                             IServiceProvider serviceProvider)
        {
            _log             = log;
            _hostLifetime    = hostLifetime;
            _serviceProvider = serviceProvider;
        }

    #endregion

    #region Methods

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for completely application is started
                    if (!_hostLifetime.ApplicationStarted.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                        continue;
                    }

                    // Get the api client
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var apiClient        = scope.ServiceProvider.GetRequiredService<NetCupDynDnsApiClient>();
                        var netCupApiOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<NetCupApi>>();
                        await apiClient.UpdateDynDns();

                        await Task.Delay(netCupApiOptions.CurrentValue.RequestInterval!.Value, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "An error occurred while updating the DynDNS records. Exception Message: {ExMessage}", ex.Message);
                }
            }
        }

    #endregion
    }
}
