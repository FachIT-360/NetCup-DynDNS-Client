// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using FachIT360.Utils.Dns.NetCup.Models.AppSettings;
using FachIT360.Utils.Dns.NetCup.Services;

using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace FachIT360.Utils.Dns.NetCup
{
    public class Program
    {
    #region Constructors and Destructors

        private Program() { }

    #endregion

    #region Methods

        public static async Task<IHost?> Bootstrap(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Logging
                   .ClearProviders()
                   .AddSimpleConsole(options =>
                                     {
                                         options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff | ";
                                         options.ColorBehavior   = LoggerColorBehavior.Disabled;
                                         options.SingleLine      = false;
                                     });

            builder.Services.Configure<NetCupApi>(builder.Configuration.GetSection("NetCupApi"));

            builder.Services.AddHttpClient<NetCupApiClient>((sp, client) =>
                                                                client.BaseAddress = sp.GetRequiredService<IOptionsMonitor<NetCupApi>>()
                                                                                       .CurrentValue.EndpointUrl);

            builder.Services.AddSingleton<WorkerTask>();
            builder.Services.AddHostedService<WorkerService>();

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            // Check Configuration
            try
            {
                host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>()
                    .OnChange(_ => logger.LogInformation("NetCupApi configuration changed."));

                _ = host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

                // Check ApiKey
                if (string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey))
                {
                    host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey =
                        builder.Configuration["NETCUP_LOGIN_APIKEY"];

                    if (string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey))
                    {
                        Environment.ExitCode = 1;
                    }
                }

                // Check ApiPassword
                if (string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword))
                {
                    host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword =
                        builder.Configuration["NETCUP_LOGIN_APIPASSWORD"];

                    if (string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword))
                    {
                        Environment.ExitCode = 1;
                    }
                }

                // Check CustomerNumber
                if (!host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber.HasValue)
                {
                    if (uint.TryParse(builder.Configuration["NETCUP_LOGIN_CUSTOMERNUMBER"], out var customerNumber))
                    {
                        host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber = customerNumber;
                    }
                    else
                    {
                        Environment.ExitCode = 1;
                    }
                }

                // Check EndpointUrl
                if (host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl == null ||
                    !host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl!.IsAbsoluteUri)
                {
                    Environment.ExitCode = 1;
                }

                logger.LogInformation("NetCupApi configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while bootstrap application.");
                await host.StopAsync();
                Environment.ExitCode = 1;

                return null;
            }

            return host;
        }

        [ExcludeFromCodeCoverage]
        public static async Task<int> Main(string[] args)
        {
            var host = await Bootstrap(args);

            if (host == null)
            {
                return Environment.ExitCode;
            }

            await host.RunAsync();

            return Environment.ExitCode;
        }

    #endregion
    }
}
