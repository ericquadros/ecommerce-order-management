﻿ENV ASPNETCORE_CULTURE=pt-BR

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EcommerceOrderManagement.EventConsumer/EcommerceOrderManagement.EventConsumer.csproj", "EcommerceOrderManagement.EventConsumer/"]
RUN dotnet restore "EcommerceOrderManagement.EventConsumer/EcommerceOrderManagement.EventConsumer.csproj"
COPY . .
WORKDIR "/src/EcommerceOrderManagement.EventConsumer"
RUN dotnet build "EcommerceOrderManagement.EventConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceOrderManagement.EventConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceOrderManagement.EventConsumer.dll"]
