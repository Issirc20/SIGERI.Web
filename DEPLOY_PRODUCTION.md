# Despliegue a Producción - SIGERI.Web

## Estado actual validado
- Compilación de solución: **OK**
- Pruebas unitarias: **33/33 OK**
- Cobertura de líneas global (último reporte): **97.6%**
- Arranque de la aplicación: **OK** en `http://localhost:5100`

## Cambios aplicados para producción
- Se limitó la inicialización de base de datos y seed a entorno **Development** (`Program.cs`).
- Se agregó `appsettings.Production.json`.
- Se ajustó el bloque de usuario en header a **botón de perfil con panel desplegable** para evitar ocupar espacio fijo.

## Configuración obligatoria antes de desplegar
1. Definir cadena de conexión de producción en variable de entorno:
   - `ConnectionStrings__DefaultConnection`
2. Definir entorno:
   - `ASPNETCORE_ENVIRONMENT=Production`
3. Confirmar conectividad SQL Server y permisos de la cuenta de aplicación.

## Validación previa al despliegue
1. `dotnet build SIGERI.Web.slnx -c Release`
2. `dotnet test SIGERI.UnitTests/SIGERI.UnitTests.csproj`
3. (Opcional cobertura manual)
   - `dotnet test SIGERI.UnitTests/SIGERI.UnitTests.csproj --collect:"XPlat Code Coverage"`

## Publicación (cuando se decida destino)
Comando base:
- `dotnet publish SIGERI.Web/SIGERI.Web.csproj -c Release -o ./artifacts/publish`

Con eso queda el artefacto listo para IIS, Azure App Service o contenedor.
