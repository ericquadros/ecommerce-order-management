ENV ASPNETCORE_CULTURE=pt-BR

#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#EXPOSE 8080
#EXPOSE 8081
ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src

# Copia apenas o arquivo .csproj e restaura as dependências
COPY ./EcommerceOrderManagement ./EcommerceOrderManagement
COPY ./EcommerceOrderManagement.WebApi ./EcommerceOrderManagement.WebApi
RUN dotnet restore "EcommerceOrderManagement.WebApi/EcommerceOrderManagement.WebApi.csproj"

# Compila a aplicação
RUN dotnet build  "EcommerceOrderManagement.WebApi/EcommerceOrderManagement.WebApi.csproj" -c "$BUILDCONFIG" --no-restore

# Publica a aplicação
RUN dotnet publish  "EcommerceOrderManagement.WebApi/EcommerceOrderManagement.WebApi.csproj" -c "$BUILDCONFIG" -o /build/



#
## Define o diretório de trabalho para a pasta da API
#WORKDIR "/app/EcommerceOrderManagement.WebApi"
#RUN dotnet build "EcommerceOrderManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "EcommerceOrderManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "EcommerceOrderManagement.WebApi.dll"]
