using Microsoft.Extensions.Logging.Abstractions;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class Worker(ILogger<Worker>? log, 
                        IHostApplicationLifetime hostLifetime, 
                        NetCupDynDnsApiClient apiClient) : BackgroundService
    {
        private readonly ILogger<Worker> _log = log ?? NullLogger<Worker>.Instance;

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            await base.StopAsync(cancellationToken);
        }

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

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
