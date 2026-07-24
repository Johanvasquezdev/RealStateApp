FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar proyectos para restaurar dependencias (caché de Docker)
COPY ["RealEstateApp/RealEstateApp.csproj", "RealEstateApp/"]
COPY ["RealEstateApp.Core.Application/RealEstateApp.Core.Application.csproj", "RealEstateApp.Core.Application/"]
COPY ["RealEstateApp.Core.Domain/RealEstateApp.Core.Domain.csproj", "RealEstateApp.Core.Domain/"]
COPY ["RealEstateApp.Infrastructure.Identity/RealEstateApp.Infrastructure.Identity.csproj", "RealEstateApp.Infrastructure.Identity/"]
COPY ["RealEstateApp.Infrastructure.Persistence/RealEstateApp.Infrastructure.Persistence.csproj", "RealEstateApp.Infrastructure.Persistence/"]
COPY ["RealEstateApp.Infrastructure.Shared/RealEstateApp.Infrastructure.Shared.csproj", "RealEstateApp.Infrastructure.Shared/"]

RUN dotnet restore "RealEstateApp/RealEstateApp.csproj"

# Copiar todo el código y construir
COPY . .
WORKDIR "/src/RealEstateApp"
RUN dotnet build "RealEstateApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RealEstateApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealEstateApp.dll"]
