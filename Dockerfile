# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY demo-api.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto por defecto de ASP.NET Core
EXPOSE 80

# Comando de inicio
ENTRYPOINT ["dotnet", "demo-api.dll"]
