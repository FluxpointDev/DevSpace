FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
USER root
WORKDIR /app
EXPOSE 5555

LABEL org.opencontainers.image.source=https://github.com/Misha-133/DevSpace


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
RUN dotnet publish "./DevSpaceAgent.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevSpaceAgent.dll"]