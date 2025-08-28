#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev libxml2
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["./NetCup_DynDNS_Client/NetCup_DynDNS_Client.csproj", "NetCup_DynDNS_Client/"]
COPY ["./NetCup_DynDNS_Client.Tests/NetCup_DynDNS_Client.Tests.csproj", "NetCup_DynDNS_Client.Tests/"]
RUN dotnet restore "./NetCup_DynDNS_Client/NetCup_DynDNS_Client.csproj"
RUN dotnet restore "./NetCup_DynDNS_Client.Tests/NetCup_DynDNS_Client.Tests.csproj"
COPY . .
WORKDIR "/src/NetCup_DynDNS_Client"

RUN --mount=type=secret,id=sonar_api_key \
    export SONAR_SECRET=$(cat /run/secrets/sonar_api_key) && \
    echo $SONAR_SECRET
RUN export PATH="$PATH:~/.dotnet/tools" &&  \
    dotnet tool install --global dotnet-sonarscanner && \
    dotnet tool install --global dotnet-coverage && \
    dotnet coverage collect "dotnet test" -f xml -o "coverage.xml" && \
    dotnet sonarscanner begin \
    /o:"fachit360" /k:"FachIT360_NetCup-DynDns-Client" \
    /d:sonar.token="$SONAR_SECRET" \
    /d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml" && \
    dotnet build "./NetCup_DynDNS_Client.csproj" -c "$BUILD_CONFIGURATION" -o /app/build && \
    dotnet sonarscanner end /d:sonar.token="$SONAR_SECRET"    

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./NetCup_DynDNS_Client.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS final
USER app
WORKDIR /app

COPY --from=publish /app/publish .

ENV NETCUP_LOGIN_APIKEY=""
ENV NETCUP_LOGIN_APIPASSWORD=""
ENV NETCUP_LOGIN_CUSTOMERNUMBER=""

ENTRYPOINT ["./netcup-dyndns"]