# SIGERI - Sistema de Gestión de Riesgos de Ciberseguridad Industrial

SIGERI es una plataforma empresarial moderna diseñada para la gestión, evaluación, tratamiento y monitoreo de riesgos de ciberseguridad en entornos industriales, alineada con marcos internacionales como **NIST CSF** e **ISO 27001**.

---

## 🛠️ Arquitectura y Tecnologías

El sistema está desarrollado bajo los principios de **Clean Architecture** (Arquitectura Limpia) y **DDD (Domain-Driven Design)**, garantizando el desacoplamiento de componentes, mantenibilidad y testabilidad.

### Estructura de Proyectos
1.  **[SIGERI.Domain](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Domain)**: Entidades de negocio, enums y objetos de valor (Value Objects). No posee dependencias externas.
2.  **[SIGERI.Application](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Application)**: Interfaces, DTOs, Casos de Uso (Command/Query con MediatR), comportamientos de tubería (Pipeline Behaviors) y validadores.
3.  **[SIGERI.Infrastructure](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Infrastructure)**: Persistencia de datos (Entity Framework Core con SQL Server), servicios de infraestructura y semilla de datos (Seed Data).
4.  **[SIGERI.Web](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web)**: Capa de presentación ASP.NET Core MVC. Interfaz moderna, responsiva y segura.
5.  **[SIGERI.UnitTests](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.UnitTests)**: Pruebas unitarias integrales de controladores, validadores y comportamientos.
6.  **[SIGERI.IntegrationTests](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.IntegrationTests)**: Pruebas de integración reales utilizando **Testcontainers** y Docker.

---

## 🚀 Guía de Inicio Rápido

### Requisitos Previos
*   [.NET SDK 10](https://dotnet.microsoft.com/)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para pruebas de integración con Testcontainers)
*   Microsoft SQL Server LocalDB o Instancia de SQL Server

### 1. Clonar y Compilar
Abre tu consola y compila la solución completa:
```bash
dotnet restore SIGERI.Web.slnx
dotnet build SIGERI.Web.slnx -c Debug
```

### 2. Configurar Base de Datos
El proyecto tiene las migraciones listas. Aplica las migraciones a tu base de datos local:
```bash
dotnet ef database update --project SIGERI.Infrastructure --startup-project SIGERI.Web
```
*Nota: Al iniciar la aplicación web por primera vez, el sistema ejecutará la semilla automática de datos (`SeedData.cs`), creando el usuario administrador default (`admin@sigeri.local` / `Admin123#`).*

### 3. Ejecutar la Aplicación Web
Inicia la capa de presentación:
```bash
dotnet run --project SIGERI.Web
```
Accede al portal en [http://localhost:5000](http://localhost:5000) o la URL indicada en consola.

---

## 🧪 Pruebas Unitarias e Integración

### Pruebas Unitarias
Ejecuta la suite de pruebas unitarias para validar las reglas de negocio y manejadores:
```bash
dotnet test SIGERI.UnitTests
```

### Pruebas de Integración con Testcontainers
Asegúrate de tener **Docker Desktop iniciado** y ejecuta:
```bash
dotnet test SIGERI.IntegrationTests
```
*Este comando descargará de forma segura la imagen oficial de SQL Server (`mcr.microsoft.com/mssql/server:2022-latest`), creará un contenedor temporal, aplicará las migraciones, correrá las pruebas y destruirá el contenedor automáticamente al finalizar.*

### Ejecutar Todas las Pruebas de la Solución
```bash
dotnet test
```

---

## 📝 Documentación del Proyecto
Para detalles específicos sobre el cumplimiento del ciclo de vida de desarrollo de software (SDLC) y el aseguramiento del desarrollo seguro (SSD), consulta el archivo de estatus técnico:
*   [PROJECT_STATUS.md](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/PROJECT_STATUS.md)
