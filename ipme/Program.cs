using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace ipme
{
    public class Program
    {
        public required string Test { get; init; }
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
                                                      {
                                                          options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
                                                          options.SerializerOptions.WriteIndented = true;
                                                      });
            
            var app = builder.Build();

            var todosApi = app.MapGroup("/v1");
            todosApi.MapGet("/", CheckRefererIp);

            app.Run();
        }
        
        private static IResult CheckRefererIp(HttpContext context)
        {
            var remoteIp   = context.Connection.RemoteIpAddress;

            if (remoteIp != null)
            {
                return Results.Text($"{remoteIp}");
            }

            var forwardedForHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedForHeader))
            {
                remoteIp = forwardedForHeader.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(ipString =>
                                              {
                                                  if (IPAddress.TryParse(ipString.Trim(), out var address) &&
                                                      address.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6)
                                                  {
                                                      return address;
                                                  }

                                                  return null;
                                              })
                                      .FirstOrDefault(x => x != null);

                if (remoteIp != null)
                {
                    return Results.Text($"{remoteIp}");
                }
                
            }

            var xRealIpHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            
            return Results.Problem("Error while trying to get the referer ip address.", null, StatusCodes.Status500InternalServerError, "Cannot get the referer ip address.");
        }
    }

    [JsonSerializable(typeof(ProblemDetails))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext { }
}
