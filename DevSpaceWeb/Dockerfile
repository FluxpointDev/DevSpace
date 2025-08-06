FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER root
WORKDIR /app

LABEL org.opencontainers.image.source=https://github.com/FluxpointDev/DevSpace
LABEL org.opencontainers.image.documentation=https://docs.fluxpoint.dev/devspace


FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevSpaceWeb/DevSpaceWeb.csproj", "DevSpaceWeb/"]
COPY ["DevSpaceServiceDefaults/DevSpaceServiceDefaults.csproj", "DevSpaceServiceDefaults/"]
COPY ["DevSpaceShared/DevSpaceShared.csproj", "DevSpaceShared/"]
RUN dotnet restore "./DevSpaceWeb/DevSpaceWeb.csproj"
COPY . .
WORKDIR "/src/DevSpaceWeb"
RUN dotnet build "./DevSpaceWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build --nologo -property WarningLevel=0


FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./DevSpaceWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --nologo -property WarningLevel=0


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apk add fontconfig curl
ENTRYPOINT ["dotnet", "DevSpaceWeb.dll"]