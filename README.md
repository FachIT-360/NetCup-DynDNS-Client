# NetCup DynDNS Client

A simple .NET 9 Service to update your NetCup DNS records over the public REST API from [NetCup GmbH](https://www.netcup.com/en/).

The focus of this client was to synchronize the IP address for individual DNS records. Currently, only 'A' records are reliably supported.   
Support for additional DNS record types will be added later.

## Getting Started

### Download

The DynDns Client is available as compressed precompiled binaries for linux-x64 and windows-x64 and can be downloaded from the [latest releases page](https://ftp.fachit360.de/netcup-dyndns/).   
After downloading you can extract the archive to a folder of your choice.

## Configuration

The easiest and fastest way to install the client is by editing the "appsettings.json" file in the main directory.

> #### <span style="color:red">&#x26A0;&#xfe0f; Note!</span><br/>  
> It is not recommended to store the API credentials in the appsetting.json on publicly accessible computers.
>
> Instead, use environment variables. In the case of a docker, use the docker secret feature. In the case of Kubernetes, use the Kubernetes secrets.  
> Bellow I'm providing two examples.

First, you need to get the API credentials from the NetCup Customer Control Panel at https://www.customercontrolpanel.de/.  
In the following screenshot you can see the location where you can get the API credentials in the NetCup Customer Control Panel.

[![NetCup Customer Control Panel](./assets/ccp-backend.png)](./assets/ccp-backend.png)

If you don't want to wait a long period of time for DNS propagation, you can set the TTL of your domain to 300 seconds.  
To check if the propagation is finished, you can use the following Google search https://www.google.com/search?q=dns+propagation, and you will find a lot of tools to check this.

After that you can enter the credentials in the appsettings.json. The following JSON snippet shows the configuration.  
You will recognize that the Login section is not present, and you have to copy them from the example. Remember to replace the placeholders with your credentials, and if you  
want to install the client later on docker or kubernetes, you should use the environment variables and remove the Login section from the appsettings.json for security reasons.  
Also, if the login section is present, the environment variables will be ignored.

Example for the appsettings.json:
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
      "ApiKey": "SqW235M4XLRPbv69XDreZ0qdWJOoC5CKmPnvn6VTogSKHQjMWX",
      "ApiPassword": "LO2t4md76hiZLYISIuPqezMYbZjnmtIIX6Nx+auzyjS34cfd1v",
      "CustomerNumber": 123456
    },
    "EndpointUrl": "https://ccp.netcup.net/run/webservice/servers/endpoint.php?JSON",
    "MyIp4ApiUrl": "https://api.ipify.org", // <-- This returns the raw public IP address of your Internet connection in verson 4.
    "MyIp6ApiUrl": "https://api6.ipify.org", // <-- This returns the raw public IP address of your Internet connection in verson 6.
    "RequestInterval": "00:05:00", // <-- The interval in which the client will check the IP address.
    "Domains": {
      "domain.com": [
        "@",
        "*",
        "subdomain1",
        "subdomain2"
      ]
    }
  }
}
````  

### Configure Domains and DNS Records

The configuration of the domains and DNS records is done in the appsettings.json.  
You define the domains and the DNS records as a collection of string that you want to update.  
If you add the '@' and '*' entries, all wildcard subdomains will point to your public IP address after synchronization.


## How run inside a Windows or Linux cli

To run the client in a linux bash or windows powershell, you have to apply the credentials Environment variables.
You can also use environment variables to configure the client. The following environment variables are supported:

* NETCUP_LOGIN_APIKEY
* NETCUP_LOGIN_APIPASSWORD
* NETCUP_LOGIN_CUSTOMERNUMBER

### Setup environment variables for direct execution on Linux

For Linux systems you can use the following bash command to set the environment variables:

```bash  
export NETCUP_LOGIN_APIKEY=<YOUR_API_KEY>
export NETCUP_LOGIN_APIPASSWORD=<YOUR_API_PASSWORD>
export NETCUP_LOGIN_CUSTOMERNUMBER=<YOUR_NETCUP_CUSTOMER_NUMBER>
```

Then run the comand inside the folder where you extract the binaries:

`./netcup-dyndns`

### Setup environment variables for direct execution on Windows

You can use the following powershell command to set the environment variables:

```powershell 
$env:NETCUP_LOGIN_APIKEY=<YOUR_API_KEY>  
$env:NETCUP_LOGIN_APIPASSWORD=<YOUR_API_PASSWORD>  
$env:NETCUP_LOGIN_CUSTOMERNUMBER=<YOUR_NETCUP_CUSTOMER_NUMBER>  
```  

Then run the comand inside the folder where you extract the binaries:

`./netcup-dyndns.exe`

## Run on Docker or Kubernetes

You can run the client in a Docker container or container orchestration platform like Kubernetes. I'm providing a Docker image of the latest release on Docker Hub.  
You will find the image here: https://hub.docker.com/repository/docker/mreinhart2805/fit360-netcup-ddns-client/general.  
The following sections will show you how to run the client on Docker or Kubernetes.

### Run as Docker Container


### Run as Kubernetes deployment

#### Prerequisite

* A ready-to-use Kubernetes Cluster
* Installed kubectl (https://kubernetes.io/docs/tasks/tools/install-kubectl/)

> #### <span style="color:red">&#x26A0;&#xfe0f; Note!</span><br/>  
> Make sure you have set the correct Kubernetes cluster context.<br/>
> To check the current context, run the following command: `kubectl config current-context`<br/>
> To list all contexts, run the following command: `kubectl config get-contexts`<br/> 
> To set the correct context, run the following command: `kubectl config use-context <context name>`

#### Add Kubernetes Namespace

First, we need a namespace. To add a namespace to your Kubernetes cluster, you can use the following command:

`kubectl create namespace network`

#### Add Kubernetes Secrets for NetCup API Credentials

For the NetCup API credentials as a Kubernetes secret, you can use the following YAML definition.

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: netcup-dns-api-secrets
  namespace: network
type: Opaque
data:
  api-key: <base64 encoded api key>
  api-password: <base64 encoded api password>
  customer-number: <base64 encoded customer number>
```

Before you exchange the placeholders for the credentials, you must base64 encode the values.

Example:<br/>
`echo -ne "I'm an API Key that is base64 encoded and used for Kubernetes secret" | base64 -w0`

Insert the encoded strings to the right property and save the file as `netcup-dns-api-secrets.yaml`.
Now we can apply the secret definition to the Kubernetes cluster. We can do this by running the following command:

`kubectl apply -f netcup-dns-api-secrets.yaml -n network`

#### Add Kubernetes ConfigMap for NetCup DNS Client Configuration (appsettings.json)

Since the appsettings.json file is immutable in the Docker image, we will create a ConfigMap with the contents of the appsettings.json file and load it as a volume in the deployment.
This also makes it possible to add or remove additional domains and / or DNS records.

The following YAML definition describes the ConfigMap. Save it as `appsettings-cm.yaml`.

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: appsettings-cm
  namespace: network
data:
  appsettings.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Debug",
          "System": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      },
      "NetCupApi": {
        "EndpointUrl": "https://ccp.netcup.net/run/webservice/servers/endpoint.php?JSON",
        "MyIp4ApiUrl": "https://api.ipify.org",
        "MyIp6ApiUrl": "https://api6.ipify.org",
        "RequestInterval": "00:05:00",
        "Domains": {
          "domain.com": [
            "@",
            "*",
            "subdomain1",
            "subdomain2"
          ]
        }
      }
    }
```

Now we can apply the ConfigMap definition to the Kubernetes cluster. We can do this by running the following command:<br/>
`kubectl apply -f appsettings-cm.yaml -n network`

#### Add Kubernetes Deployment

Last but not least, there is the Kubernetes deployment, which connects everything and executes it after application.
The following YAML definition describes the deployment. Save it as `netcup-dns-deployment.yaml`.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fit360-netcup-ddns-client
  namespace: network
  labels:
    app: fit360-netcup-ddns-client
spec:
  replicas: 1
  selector:
    matchLabels:
      app: fit360-netcup-ddns-client
  template:
    metadata:
      labels:
        app: fit360-netcup-ddns-client
    spec:
      volumes:
        - name: appsettings-volume
          configMap:
            name: appsettings-cm
      containers:
        - name: fit360-netcup-ddns-client
          image: >-
            registry.fachit360.de/network/fit360-netcup-ddns-client:1.1.14-4aecb46ab1-amd64
          volumeMounts:
            - name: appsettings-volume
              mountPath: /app/appsettings.json
              subPath: appsettings.json
              readOnly: true
          env:
            - name: NETCUP_LOGIN_APIKEY
              valueFrom:
                secretKeyRef:
                  name: netcup-dns-api-secrets
                  key: api-key
            - name: NETCUP_LOGIN_APIPASSWORD
              valueFrom:
                secretKeyRef:
                  name: netcup-dns-api-secrets
                  key: api-password
            - name: NETCUP_LOGIN_CUSTOMERNUMBER
              valueFrom:
                secretKeyRef:
                  name: netcup-dns-api-secrets
                  key: customer-number
          resources:
            requests:
              memory: "64Mi"
              cpu: "100m"
            limits:
              memory: "256Mi"
 ```

# Run Tests

* Der Test ist abhängig vom NetCup API Service

`dotnet test -e NETCUP_LOGIN_APIKEY="SqW235M4XLRPbv69XDreZ0qdWJOoC5CKmPnvn6VTogSKHQjMWX" -e NETCUP_LOGIN_APIPASSWORD="LO2t4md76hiZLYISIuPqezMYbZjnmtIIX6Nx+auzyjS34cfd1v" -e NETCUP_LOGIN_CUSTOMERNUMBER="123456" --collect "XPlat Code Coverage;Format=opencover"`

# Known Issues

IPv6

#### Support my work

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/J3J01JYMKA)
