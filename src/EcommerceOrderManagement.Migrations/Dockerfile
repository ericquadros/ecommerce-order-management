FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EcommerceOrderManagement.Migrations/EcommerceOrderManagement.Migrations.csproj", "EcommerceOrderManagement.Migrations/"]
RUN dotnet restore "EcommerceOrderManagement.Migrations/EcommerceOrderManagement.Migrations.csproj"
COPY . .
WORKDIR "/src/EcommerceOrderManagement.Migrations"
RUN dotnet build "EcommerceOrderManagement.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceOrderManagement.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceOrderManagement.Migrations.dll"]
