using System.Diagnostics.CodeAnalysis;
using System.Net;

using FachIT360.Utils.Dns.NetCup.Models.Settings;
using FachIT360.Utils.Dns.NetCup.Services;

using Microsoft.Extensions.Logging.Console;

namespace FachIT360.Utils.Dns.NetCup
{
    public class Program
    {
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
                                         options.SingleLine = true;
                                     });
            
            builder.Services.Configure<NetCupApi>(builder.Configuration.GetSection("NetCupApi"));
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
    }
}