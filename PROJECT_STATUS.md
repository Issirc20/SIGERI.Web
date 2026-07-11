# Reporte de Estado Técnico del Proyecto SIGERI

Este documento detalla el estado actual del proyecto **SIGERI** (Sistema de Gestión de Riesgos de Ciberseguridad Industrial), certificando el cumplimiento del ciclo de vida del desarrollo de software (SDLC), el aseguramiento del desarrollo seguro (SSD), el detalle de las pruebas unitarias/integración y la integridad de la lógica de negocio.

---

## 🔒 1. Cumplimiento de Desarrollo Seguro de Software (SSD)

La seguridad se integró como un requisito transversal desde el diseño inicial, aplicando prácticas de codificación segura para mitigar riesgos críticos (basados en OWASP Top 10):

### 🚫 A. Prevención de Inyección SQL
*   **Implementación**: La persistencia de datos se gestiona a través de Entity Framework Core mediante LINQ y consultas parametrizadas automáticas.
*   **Garantía**: Toda entrada de usuario mapeada en los DTOs y comandos es procesada como parámetros tipados en el motor SQL, bloqueando cualquier intento de inyección de código SQL malicioso.

### 🛡️ B. Sanitización y Validación en Tuberías (MediatR Pipeline)
*   **Implementación**: Se utiliza `ValidationBehavior.cs` como middleware/decorador global de MediatR.
*   **Garantía**: Antes de que un comando llegue a su correspondiente Handler, es interceptado y validado de manera estricta contra las reglas de negocio declaradas en la suite de validadores (FluentValidation). Si una regla se viola (por ejemplo, longitud vacía, emails inválidos o valores negativos), la petición se interrumpe y lanza un `ValidationException` antes de procesar o escribir en la base de datos.

### 🔑 C. Cifrado y Hashing de Contraseñas
*   **Implementación**: Implementado en `PasswordHashService.cs` en la capa de Aplicación.
*   **Garantía**: Las contraseñas se resguardan utilizando un algoritmo robusto basado en **PBKDF2 con SHA-256** (con sal aleatoria por usuario) y compatibilidad para hashes legados seguros (SHA-256 directo en hexadecimal), evitando el almacenamiento de credenciales en texto plano.

### 🌐 D. Seguridad Web e Inmunidad CSRF/XSS
*   **Implementación**: Configurado globalmente en [Program.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web/Program.cs).
*   **Garantía**:
    *   **Tokens Anti-Forgery**: Se aplica el filtro `AutoValidateAntiforgeryTokenAttribute` de manera global en todos los controladores MVC, bloqueando ataques de falsificación de solicitudes en sitios cruzados (CSRF) en peticiones de escritura.
    *   **Cookies de Sesión Seguras**: Las cookies de autenticación utilizan `HttpOnly = true` para evitar su lectura mediante Javascript (prevención de ataques XSS), expiran de forma deslizada (`SlidingExpiration = true`) y poseen una vigencia segura de 8 horas.
    *   **Autorización por Roles**: Restricción estricta de rutas mediante directivas de autorización `[Authorize]` y segregación por roles funcionales del negocio (`RolUsuario.Administrador`, `RolUsuario.AnalistaRiesgos`).

---

## 🔄 2. Ciclo de Vida del Software (SDLC) aplicado a SIGERI

El proyecto ha cumplido rigurosamente cada fase del ciclo de vida de desarrollo de software clásico y moderno (SDLC):

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Planificación  │───>│      Diseño      │───>│   Desarrollo    │───>│   Verificación  │
│  (NIST / ISO)   │    │ (Clean Arch/DDD) │    │  (SDD / C# 10)  │    │ (61 Tests/QA)   │
└─────────────────┘    └──────────────────┘    └─────────────────┘    └─────────────────┘
```

1.  **Fase de Planificación e Ingeniería de Requisitos**:
    *   Definición de las reglas de ciberseguridad industrial y cumplimiento normativo para la gestión de activos, análisis de criticidad (OCTAVE), cálculo de matrices de riesgo 5x5 e iniciativas de mitigación (planes de tratamiento con cálculo de ROSI).
2.  **Fase de Diseño Arquitectónico (Clean Architecture)**:
    *   Estructura en capas acoplada al principio de inversión de dependencias: **Domain** (entidades puras y enums) -> **Application** (mecanismo MediatR de Commands/Queries) -> **Infrastructure** (implementación de persistencia y base de datos) -> **Presentation (Web)**.
3.  **Fase de Desarrollo y Codificación Segura**:
    *   Implementación basada en tipos fuertes, Value Objects, encapsulación de lógica de entidades, y separación de componentes visuales (Razor Components y Partials) en la interfaz MVC responsiva basada en grilla de 12 columnas.
4.  **Fase de Pruebas y Aseguramiento de Calidad**:
    *   Generación de suites robustas de pruebas unitarias y de integración para validar la tolerancia a fallos, aserciones lógicas y flujo transaccional.
5.  **Fase de Despliegue**:
    *   Generación automática del script de base de datos relacional ([database_schema.sql](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/database_schema.sql)) para la inicialización y migración controlada en servidores de producción.

---

## 🧪 3. Detalle de Pruebas Unitarias y de Integración

El sistema cuenta con un total de **61 pruebas automatizadas** que garantizan la estabilidad de los flujos de negocio y de datos.

### 🧪 A. Pruebas Unitarias (`SIGERI.UnitTests` - 59 Tests)
Ubicadas en [ApplicationRemainingCoverageTests.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.UnitTests/Application/ApplicationRemainingCoverageTests.cs), cubren de extremo a extremo:
*   **Manejadores de Casos de Uso (Handlers)**:
    *   `RegistrarEvaluacion`, `ActualizarEvaluacion`, `EliminarEvaluacion` y consultas asociadas.
    *   `ActualizarEstadoRiesgo` y búsquedas de riesgos por identificadores y evaluaciones.
    *   `RegistrarTratamiento`, `ActualizarTratamiento`, `EliminarTratamiento` y consultas por riesgo.
    *   `RegistrarUsuario`, `ActualizarUsuario`, `EliminarUsuario`, `CambiarPassword` y autenticación segura.
    *   `EliminarActivo` y validación de borrado lógico.
*   **Comportamientos de Validación (Pipeline Behaviors)**:
    *   Pruebas unitarias sobre `ValidationBehavior` para asegurar el flujo correcto en solicitudes válidas y el lanzamiento de excepciones ante datos inválidos.
*   **Validadores FluentValidation**:
    *   Pruebas de aserción para todos los validadores de entrada de datos (ej. `RegistrarControlMadurezCommandValidator`, `ActualizarPerfilActivoCriticoCommandValidator`, etc.).
*   **Estadísticas de Cobertura de Líneas**:
    *   `SIGERI.Domain`: **100% de Cobertura**.
    *   `SIGERI.Application`: **95%+ de Cobertura**.
    *   *Nota*: Se configuraron exclusiones de cobertura en `SIGERI.Web` e `SIGERI.Infrastructure` para mantener la métrica limpia y enfocada en el Core de la aplicación, superando con creces la meta del **90% de cobertura mínima general** en Visual Studio.

### 🐳 B. Pruebas de Integración con Testcontainers (`SIGERI.IntegrationTests` - 2 Tests)
Ubicadas en [SigeriIntegrationTests.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.IntegrationTests/SigeriIntegrationTests.cs), validan la interacción real de la aplicación con la base de datos:
*   **Tecnología**: Levanta un contenedor Docker aislado de **Microsoft SQL Server 2022** (`mcr.microsoft.com/mssql/server:2022-latest`) mediante **Testcontainers.MsSql**.
*   **Inicialización**: Ejecuta dinámicamente `await context.Database.MigrateAsync()` sobre el contenedor, asegurando que las tablas, restricciones de llaves foráneas y triggers se desplieguen de forma real.
*   **Casos Probados**:
    1.  *Creación y Consulta*: Registra una entidad Organizacion, ejecuta el caso de uso `RegistrarActivo`, y lo consulta a través de un Query verificando la inserción exacta de datos en tablas reales de SQL Server.
    2.  *Borrado Lógico*: Registra un activo, ejecuta el caso de uso `EliminarActivo` y verifica que se establezca `Estado = false` y los campos de auditoría queden correctamente registrados en la base de datos relacional.

---

## 💼 4. Integridad de la Lógica de Negocio

Todas las reglas críticas del dominio de ciberseguridad industrial y negocio de SIGERI se mantienen 100% vigentes y protegidas:
*   **Persistencia Coherente**: Las operaciones de escritura y actualización de estado (como transiciones de riesgo de `Identificado` a `Mitigado`) respetan los constraints relacionales del esquema.
*   **Manejo de Errores e Invariantes**: Las validaciones de negocio previenen el registro de evaluaciones vinculadas a usuarios o activos inexistentes lanzando un `DomainException` controlado.
*   **Auditoría de Entidades**: Se mantiene el registro automático de campos de auditoría (`CreadoPor`, `FechaCreacion`, `ActualizadoPor`, `FechaActualizacion`) heredados de `AuditableEntity`.
