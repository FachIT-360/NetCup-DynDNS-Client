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