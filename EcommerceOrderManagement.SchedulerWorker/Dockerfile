FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EcommerceOrderManagement.SchedulerWorker/EcommerceOrderManagement.SchedulerWorker.csproj", "EcommerceOrderManagement.SchedulerWorker/"]
RUN dotnet restore "EcommerceOrderManagement.SchedulerWorker/EcommerceOrderManagement.SchedulerWorker.csproj"
COPY . .
WORKDIR "/src/EcommerceOrderManagement.SchedulerWorker"
RUN dotnet build "EcommerceOrderManagement.SchedulerWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceOrderManagement.SchedulerWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceOrderManagement.SchedulerWorker.dll"]
