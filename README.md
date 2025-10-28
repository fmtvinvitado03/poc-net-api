# poc-net-api

Documento técnico para levantar y desarrollar el entorno de la API .NET incluida en este repositorio.

> Nota: este README proporciona instrucciones genéricas y prácticas recomendadas para proyectos .NET + Docker. Si el repositorio contiene un `global.json`, `docker-compose.yml` u otros archivos de configuración, verifica las versiones y rutas allí antes de seguir las instrucciones.

## Contenido
- Requisitos previos
- Configuración local (dotnet)
- Ejecutar con Docker / Docker Compose
- Variables de entorno y configuración
- Migraciones de base de datos (EF Core)
- Ejecutar pruebas
- Depuración (VS Code / Visual Studio)
- Formato y linting
- Solución de problemas comunes
- Contribuir

---

## Requisitos previos

Instala lo siguiente en tu máquina de desarrollo:

- .NET SDK 7.0 o superior (recomendado .NET 8 si es compatible con el proyecto)
  - Comprobar versión instalada:
    ```
    dotnet --version
    ```
  - Si existe `global.json` en repo, usa la versión indicada ahí.
- Git
- Docker Engine y Docker Compose (si vas a usar contenedores)
- (Opcional) Visual Studio 2022/2023 o Visual Studio Code con la extensión C# (OmniSharp)
- (Opcional) EF Core tools si usas migrations:
  ```
  dotnet tool install --global dotnet-ef
  ```

---

## Configuración del entorno local (sin Docker)

1. Clona el repositorio:
   ```
   git clone https://github.com/fmtvinvitado03/poc-net-api.git
   cd poc-net-api
   ```

2. Restaurar paquetes:
   ```
   dotnet restore
   ```

3. Construir:
   ```
   dotnet build
   ```

4. Ejecutar la API:
   - Si hay una solución (`*.sln`), puedes ejecutar desde la carpeta raíz apuntando al proyecto:
     ```
     dotnet run --project ./src/NombreDelProyectoApi/NombreDelProyectoApi.csproj
     ```
   - O ejecutar desde el directorio del proyecto:
     ```
     cd src/NombreDelProyectoApi
     dotnet run
     ```

5. Por defecto ASP.NET Core expone puertos:
   - HTTP: 5000
   - HTTPS: 5001
   Comprueba la salida de `dotnet run` para confirmar los puertos exactos.

---

## Ejecutar con Docker

Si el repositorio contiene `Dockerfile`/`docker-compose.yml`, puedes ejecutar en contenedor:

1. Construir imagen:
   ```
   docker build -t poc-net-api:dev .
   ```

2. Ejecutar contenedor:
   ```
   docker run --rm -it -p 5000:80 -p 5001:443 --env-file .env poc-net-api:dev
   ```
   Ajusta los puertos según el `Dockerfile`/la configuración de la app.

3. Usando Docker Compose:
   - Si existe `docker-compose.yml`:
     ```
     docker compose up --build
     ```
   - Para levantar en background:
     ```
     docker compose up -d
     ```

4. Ver logs:
   ```
   docker compose logs -f
   ```

---

## Variables de entorno y configuración

La aplicación probablemente use `appsettings.json` y `appsettings.Development.json`. También es común utilizar un archivo `.env` para Docker. Crea un archivo `.env` en la raíz con ejemplos como:

```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Database=poc_db;User Id=sa;Password=Your_password123;
ASPNETCORE_URLS=http://+:5000;https://+:5001
```

- Sustituye la cadena de conexión por la que uses (Postgres, SQL Server, MySQL, etc).
- No subas credenciales reales al repositorio.

---

## Migraciones y base de datos (si aplica - EF Core)

Si el proyecto usa Entity Framework Core, los comandos comunes son:

- Añadir migración:
  ```
  dotnet ef migrations add NombreDeLaMigracion --project ./src/NombreDelProyectoApi/ --startup-project ./src/NombreDelProyectoApi/
  ```

- Aplicar migraciones (migrar la base de datos):
  ```
  dotnet ef database update --project ./src/NombreDelProyectoApi/ --startup-project ./src/NombreDelProyectoApi/
  ```

- Si usas contenedores, asegúrate de que el contenedor de BD esté accesible y las variables de entorno apunten correctamente.

---

## Ejecutar pruebas

Si hay proyectos de test:

```
dotnet test
```

Para ejecutar un test concreto o en un proyecto específico:
```
dotnet test ./tests/Nombre.Tests/Nombre.Tests.csproj
```

---

## Depuración

### Visual Studio Code
1. Instala extensiones: C# (OmniSharp), Docker (opcional).
2. Abre la carpeta del repo en VSCode.
3. Ejecuta desde el menú "Run and Debug", o crea/ajusta `.vscode/launch.json` con una configuración para .NET Core. Ejemplo básico:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/NombreDelProyectoApi/bin/Debug/net8.0/NombreDelProyectoApi.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/NombreDelProyectoApi",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    }
  ]
}
```

Ajusta `net8.0`, rutas y nombres según el proyecto.

### Visual Studio
- Abrir la solución `.sln`, establecer proyecto de inicio y presionar F5.

### Depuración dentro de contenedor
- Usa la extensión Docker o configura remote debugging según las guías oficiales de Microsoft si necesitas depurar dentro de un contenedor.

---

## Formato y linting

- Formatear con `dotnet format`:
  ```
  dotnet tool install -g dotnet-format
  dotnet format
  ```
- Mantén un `.editorconfig` en la raíz para reglas compartidas.

---

## Problemas comunes

- Error de versión de SDK: comprueba `dotnet --version` y `global.json`.
- Conexión a la base de datos desde Docker: revisa las redes de Docker, variables de entorno y que el contenedor de DB esté listo (puede requerir wait-for o healthcheck).
- Permisos de puertos en macOS/Linux: usa sudo o asigna puertos > 1024 si hay conflicto.
- Certificados HTTPS en contenedores: para desarrollo, puedes desactivar HTTPS o configurar certificados dev.

---

## Buenas prácticas y recomendaciones

- No subir secretos al repo. Usa secretos de GitHub Actions / variables de entorno / vaults.
- Incluye un `global.json` si quieres forzar una versión específica del SDK.
- Mantén `Dockerfile` y `docker-compose.yml` para entornos reproducibles.
- Añade pipelines CI (GitHub Actions) para build/test/scan.

---

## Contribuir

1. Fork y branch con nombre descriptivo:
   ```
   git checkout -b feat/nueva-funcionalidad
   ```
2. Hacer cambios, añadir tests.
3. Abrir PR describiendo los cambios y cómo probarlos.

---

## Contacto

Para dudas o problemas, abre un issue en el repositorio con:
- Pasos para reproducir
- Logs relevantes
- Versión de .NET (`dotnet --version`)
- Sistema operativo

---

Fin del README.
