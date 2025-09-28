# Dockerfile para RealEstate API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7288

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["RealEstate.Api/RealEstate.Api.csproj", "RealEstate.Api/"]
COPY ["RealEstate.Application/RealEstate.Application.csproj", "RealEstate.Application/"]
COPY ["RealEstate.Infrastructure/RealEstate.Infrastructure.csproj", "RealEstate.Infrastructure/"]
COPY ["RealEstate.Domain/RealEstate.Domain.csproj", "RealEstate.Domain/"]
COPY ["RealEstate.Shared/RealEstate.Shared.csproj", "RealEstate.Shared/"]

RUN dotnet restore "RealEstate.Api/RealEstate.Api.csproj"

# Copiar todo el código fuente
COPY . .

# Build de la aplicación
WORKDIR "/src/RealEstate.Api"
RUN dotnet build "RealEstate.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealEstate.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variables de entorno por defecto
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:7288

ENTRYPOINT ["dotnet", "RealEstate.Api.dll"]
