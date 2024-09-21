ENV ASPNETCORE_CULTURE=pt-BR

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EcommerceOrderManagement.WebApi/EcommerceOrderManagement.WebApi.csproj", "EcommerceOrderManagement.WebApi/"]
RUN dotnet restore "EcommerceOrderManagement.WebApi/EcommerceOrderManagement.WebApi.csproj"
COPY . .
WORKDIR "/src/EcommerceOrderManagement.WebApi"
RUN dotnet build "EcommerceOrderManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceOrderManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceOrderManagement.WebApi.dll"]
