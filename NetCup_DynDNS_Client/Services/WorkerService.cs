// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    [ExcludeFromCodeCoverage]
    public class WorkerService : BackgroundService
    {
    #region Constants - Static fields - Fields

        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly ILogger<WorkerService>   _log;
        private readonly WorkerTask               _workerTask;

    #endregion

    #region Constructors and Destructors

        public WorkerService(ILogger<WorkerService> log, IHostApplicationLifetime hostLifetime, WorkerTask workerTask)
        {
            _log          = log;
            _hostLifetime = hostLifetime;
            _workerTask   = workerTask;
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

                    await _workerTask.StartSyncDnsRecordsAsync(stoppingToken);
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
