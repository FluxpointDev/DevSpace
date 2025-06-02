FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER root
WORKDIR /app
EXPOSE 5555

LABEL org.opencontainers.image.source=https://github.com/FluxpointDev/DevSpace
LABEL org.opencontainers.image.documentation=https://docs.fluxpoint.dev/devspace

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevSpaceAgent/DevSpaceAgent.csproj", "DevSpaceAgent/"]
COPY ["DevSpaceShared/DevSpaceShared.csproj", "DevSpaceShared/"]
RUN dotnet restore "./DevSpaceAgent/DevSpaceAgent.csproj"
COPY . .
WORKDIR "/src/DevSpaceAgent"
RUN dotnet build "./DevSpaceAgent.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN wget -O "docker.tgz" "https://download.docker.com/linux/static/stable/x86_64/docker-27.0.3.tgz"
RUN tar -xf docker.tgz docker

RUN mv docker/docker /usr/bin/docker

RUN wget -O "docker-compose" "https://github.com/docker/compose/releases/download/v2.36.2/docker-compose-linux-x86_64"
RUN mv docker-compose /usr/bin/docker-compose
RUN dotnet publish "./DevSpaceAgent.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app

RUN apt-get update

COPY --from=publish /usr/bin/docker /usr/bin/docker

COPY --from=publish /usr/bin/docker-compose /usr/local/lib/docker/cli-plugins/docker-compose
RUN chmod +x /usr/local/lib/docker/cli-plugins/docker-compose
RUN ln -sf /usr/local/lib/docker/cli-plugins/docker-compose /usr/local/bin/docker-compose
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevSpaceAgent.dll"]