// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Net;

using FachIT360.Utils.Dns.NetCup.Services;

using Microsoft.Extensions.Logging.Console;

namespace FachIT360.Utils.Dns.NetCup
{
    public class Program
    {
    #region Methods

        public static void Main(string[] args)
        {
            // Check necessary environment variables exists

            var builder = Host.CreateApplicationBuilder(args);

            builder.Logging
                   .ClearProviders()
                   .AddSimpleConsole(options =>
                                     {
                                         options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff | ";
                                         options.ColorBehavior   = LoggerColorBehavior.Default;
                                         options.SingleLine      = false;
                                     });

            builder.Services.AddHttpClient<NetCupDynDnsApiClient>()
                   .ConfigureHttpClient(client =>
                                        {
                                            client.DefaultRequestVersion = HttpVersion.Version20;
                                            client.BaseAddress           = new Uri("https://ccp.netcup.net/run/webservice/servers/endpoint.php?JSON");
                                        });

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();

            host.Run();
        }

    #endregion
    }
}
