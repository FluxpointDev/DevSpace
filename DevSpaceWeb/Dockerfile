FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER root
WORKDIR /app
EXPOSE 5556


LABEL org.opencontainers.image.source=https://github.com/FluxpointDev/DevSpace


FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevSpaceWeb/DevSpaceWeb.csproj", "DevSpaceWeb/"]
COPY ["DevSpaceServiceDefaults/DevSpaceServiceDefaults.csproj", "DevSpaceServiceDefaults/"]
COPY ["DevSpaceShared/DevSpaceShared.csproj", "DevSpaceShared/"]
RUN dotnet restore "./DevSpaceWeb/DevSpaceWeb.csproj"
COPY . .
WORKDIR "/src/DevSpaceWeb"
RUN dotnet build "./DevSpaceWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./DevSpaceWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get install -y fontconfig curl
RUN ln -s /app/DataProtection-Keys /Data/DataProtection-Keys
ENTRYPOINT ["dotnet", "DevSpaceWeb.dll"]