FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AlreadyMedia/AlreadyMedia.csproj", "AlreadyMedia/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "AlreadyMedia/AlreadyMedia.csproj"
COPY . .
WORKDIR "/src/AlreadyMedia"
RUN dotnet build "./AlreadyMedia.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AlreadyMedia.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlreadyMedia.dll"]
