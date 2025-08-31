# NetCup DynDNS Client

A simple .NET Core Service to update your NetCup DNS records.

[![Build Status](https://dev.azure.com/Fachit360/NetCup%20DynDns%20Client/_apis/build/status%2FNetCup-DynDns-Client?branchName=develop)](https://dev.azure.com/Fachit360/NetCup%20DynDns%20Client/_build/latest?definitionId=2&branchName=develop) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client) [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client) [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=coverage)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=FachIT360_NetCup-DynDns-Client&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=FachIT360_NetCup-DynDns-Client)

# appsettings.json

`RequestInterval` default is five minutes

````json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "NetCupApi": {
    "Login": {
      "ApiKey": "YlM5N0E4NDEyWUY1NzZzUnI5MzFtNkg1NEIySTMybXQ4YVR2bz",
      "ApiPassword": "D2EMHeD1d3hVdRozd2Y8LsJreFFO4FNKTWVGYkS+yERybEOJSN",
      "CustomerNumber": 163189
    },
    "EndpointUrl": "https://ccp.netcup.net/run/webservice/servers/endpoint.php?JSON",
    "MyIp4ApiUrl": "https://api.ipify.org",
    "MyIp6ApiUrl": "https://api6.ipify.org",
    "RequestInterval": "00:05:00",
    "Domains": {
      "fachit360.de": [
        "@",
        "*"
      ]
    }
  }
}
````

# Enviroment variables

NETCUP_LOGIN_APIKEY

NETCUP_LOGIN_APIPASSWORD

NETCUP_LOGIN_CUSTOMERNUMBER

# Run Tests

* Der Test ist abhängig vom NetCup API Service

`dotnet test -e NETCUP_LOGIN_APIKEY="YlM5N0E4NDEyWUY1NzZzUnI5MzFtNkg1NEIySTMybXQ4YVR2bz" -e NETCUP_LOG
IN_APIPASSWORD="D2EMHeD1d3hVdRozd2Y8LsJreFFO4FNKTWVGYkS+yERybEOJSN" -e NETCUP_LOGIN_CUSTOMERNUMBER="163189" --collect "XPlat Code Coverage;Format=opencover"`