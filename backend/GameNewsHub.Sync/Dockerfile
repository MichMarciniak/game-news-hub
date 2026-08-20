FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GameNewsHub.Sync/GameNewsHub.Sync.csproj", "GameNewsHub.Sync/"]
RUN dotnet restore "GameNewsHub.Sync/GameNewsHub.Sync.csproj"
COPY . .
WORKDIR "/src/GameNewsHub.Sync"
RUN dotnet build "./GameNewsHub.Sync.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GameNewsHub.Sync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameNewsHub.Sync.dll"]
