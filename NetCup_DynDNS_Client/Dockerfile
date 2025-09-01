FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["./NetCup_DynDNS_Client/NetCup_DynDNS_Client.csproj", "NetCup_DynDNS_Client/"]
RUN dotnet restore "./NetCup_DynDNS_Client/NetCup_DynDNS_Client.csproj"
COPY . .
WORKDIR "/src/NetCup_DynDNS_Client"

RUN dotnet build "./NetCup_DynDNS_Client.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

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