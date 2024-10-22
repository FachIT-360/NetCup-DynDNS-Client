#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["DynDNS_4_NetCup_DotNet/DynDNS_4_NetCup_DotNet.csproj", "DynDNS_4_NetCup_DotNet/"]
RUN dotnet restore "./DynDNS_4_NetCup_DotNet/DynDNS_4_NetCup_DotNet.csproj"
COPY . .
WORKDIR "/src/DynDNS_4_NetCup_DotNet"
RUN dotnet build "./DynDNS_4_NetCup_DotNet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./DynDNS_4_NetCup_DotNet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app

RUN mkdir -p /root/.ssh && \
    chmod 700 /root/.ssh

COPY --from=publish /app/publish .
COPY --from=publish /app/publish/authorized_keys /root/.ssh/authorized_keys

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    openssh-server nano wget curl mc

RUN chmod 600 /root/.ssh/authorized_keys
RUN sed -i -e 's/#Port 22/Port 2300/g' /etc/ssh/sshd_config
RUN mkdir -p ~/.vs-debugger && \
    wget https://aka.ms/getvsdbgsh -O ~/.vs-debugger/GetVsDbg.sh && \
    chmod +x ~/.vs-debugger/GetVsDbg.sh

ENV NETCUP_LOGIN_APIKEY= 
ENV NETCUP_LOGIN_APIPASSWORD=
ENV NETCUP_LOGIN_CUSTOMERNUMBER=

EXPOSE 2300

ENTRYPOINT ["./netcup-dyndns"]