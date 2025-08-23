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
    internal class Program
    {
    #region Constructors and Destructors

        protected Program() { }

    #endregion

    #region Methods

        [UnconditionalSuppressMessage(
            "AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public static async Task<int> Main(string[] args)
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
            builder.Services.AddHttpClient<NetCupDynDnsApiClient>();
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
                host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey =
                    !string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey)
                        ? host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey
                        : !string.IsNullOrWhiteSpace(builder.Configuration["NETCUP_LOGIN_APIKEY"])
                            ? builder.Configuration["NETCUP_LOGIN_APIKEY"]
                            : throw new ArgumentNullException(nameof(builder.Configuration), "The ApiKey parameter in the config is not set.");

                // Check ApiPassword
                host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword =
                    !string.IsNullOrWhiteSpace(host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword)
                        ? host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword
                        : !string.IsNullOrWhiteSpace(builder.Configuration["NETCUP_LOGIN_APIPASSWORD"])
                            ? builder.Configuration["NETCUP_LOGIN_APIPASSWORD"]
                            : throw new ArgumentNullException(nameof(builder.Configuration), "The ApiPassword parameter in the config is not set.");

                // Check CustomerNumber
                host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber =
                    host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber.HasValue
                        ? host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber
                        : uint.TryParse(builder.Configuration["NETCUP_LOGIN_CUSTOMERNUMBER"], out var customerNumber)
                            ? customerNumber
                            : throw new ArgumentNullException("NETCUP_LOGIN_CUSTOMERNUMBER",
                                                              "The NetCup CustomerNumber in the config is not set correctly.");

                // Check EndpointUrl
                host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl =
                    host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl != null &&
                    host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl!.IsAbsoluteUri
                        ? host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.EndpointUrl
                        : throw new ArgumentNullException(nameof(NetCupApi.EndpointUrl),
                                                          "The NetCup EndpointUrl in the config is not set correctly.");

                logger.LogInformation("NetCupApi configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while loading NetCupApi configuration.");
                await host.StopAsync();
                Environment.ExitCode = 1;

                return Environment.ExitCode;
            }

            await host.RunAsync();

            return Environment.ExitCode;
        }

    #endregion
    }
}
