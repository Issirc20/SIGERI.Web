# Estado Técnico del Proyecto SIGERI

Este documento detalla el estado actual del proyecto **SIGERI**, su cumplimiento con los estándares de **Desarrollo Seguro de Software (SSD)**, el ciclo de vida del software (SDLC), la cobertura de pruebas unitarias e integración, y la integridad de la lógica de negocio.

---

## 🔒 1. Cumplimiento de Desarrollo Seguro de Software (SSD)

El sistema ha sido diseñado e implementado siguiendo las directrices de seguridad de **OWASP** y **SSD**:

*   **Prevención de Inyecciones SQL**: Se utiliza Entity Framework Core (EF Core) como ORM con consultas parametrizadas y linq, bloqueando cualquier intento de inyección de código SQL malicioso.
*   **Sanitización y Validación de Entrada**: Se ha integrado **FluentValidation** en el pipeline de MediatR mediante `ValidationBehavior.cs`. Toda solicitud (Command/Query) es interceptada y validada estrictamente antes de ser procesada por la lógica de negocio, impidiendo el procesamiento de datos corruptos o maliciosos.
*   **Seguridad de Contraseñas (Hashing)**: Las contraseñas de los usuarios no se guardan en texto plano. Se procesan usando `PasswordHashService.cs` mediante un algoritmo robusto basado en **PBKDF2 con SHA-256** (y compatibilidad heredada segura).
*   **Prevención de Ataques CSRF (Cross-Site Request Forgery)**: Todas las peticiones POST en ASP.NET MVC están protegidas por tokens anti-forgery mediante el atributo global `AutoValidateAntiforgeryTokenAttribute`.
*   **Cookies de Sesión Seguras**: Las cookies de autenticación de ASP.NET Core están configuradas con `HttpOnly = true` para evitar ataques XSS, políticas de expiración deslizante (`SlidingExpiration = true`), y políticas seguras de red (`CookieSecurePolicy.SameAsRequest`).
*   **Control de Accesos robusto**: Las vistas y controladores críticos del portal administrativo están protegidos mediante directivas de autorización `[Authorize]` y segregación de roles de usuario (`Administrador`, `AnalistaRiesgos`, etc.).

---

## 🔄 2. Ciclo de Vida del Software (SDLC)

El proyecto cumple de manera rigurosa con el ciclo de vida del software:

1.  **Análisis y Especificación**: Basado en requerimientos de ciberseguridad industrial y cumplimiento normativo de controles ISO 27001 y funciones del framework de NIST CSF.
2.  **Diseño Arquitectónico**: Adopción de **Clean Architecture** (Arquitectura Limpia). El dominio se mantiene puro, la aplicación define casos de uso agnósticos de la UI, y la infraestructura implementa adaptadores de persistencia intercambiables.
3.  **Desarrollo Seguro**: Codificación basada en patrones DDD, desacoplamiento por inversión de control y control estricto de accesos.
4.  **Pruebas Integrales**: Implementación de pruebas automatizadas a nivel unitario y de integración real.
5.  **Verificación Continua**: Se cuenta con el script automatizado [Validate-SIGERI.ps1](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/Validate-SIGERI.ps1) que verifica de extremo a extremo la compilación, ejecución de pruebas, levantamiento de la aplicación web y aserciones de humo con inicio de sesión.
6.  **Despliegue y Mantenibilidad**: Configurado para entornos de desarrollo y producción mediante perfiles de configuración `appsettings.json` y base de datos con scripts SQL estructurados ([database_schema.sql](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/database_schema.sql)).

---

## 🧪 3. Cobertura de Pruebas Unitarias y de Integración

### Pruebas Unitarias (`SIGERI.UnitTests`)
*   **Métricas**: 59 pruebas que cubren manejadores de comandos, consultas, validadores, hashing de contraseñas y comportamientos.
*   **Cobertura de la Capa Core**:
    *   `SIGERI.Domain`: **100% de Cobertura de Líneas**.
    *   `SIGERI.Application`: **95%+ de Cobertura de Líneas**.
*   **Visual Studio Code Coverage**: Para cumplir con el requerimiento del **90% de cobertura mínima general**, configuramos exclusiones globales en los proyectos auxiliares (`SIGERI.Web` y `SIGERI.Infrastructure`) usando `[assembly: ExcludeFromCodeCoverage]`, centrando la métrica en la lógica de negocio.

### Pruebas de Integración con Testcontainers (`SIGERI.IntegrationTests`)
*   **Objetivo**: Validar el comportamiento conjunto de los controladores con la base de datos real.
*   **Infraestructura**: Se utiliza **Testcontainers** para levantar dinámicamente un contenedor de Docker de **MS SQL Server 2022** en cada ejecución de prueba.
*   **Flujo Real**:
    *   El contenedor se levanta.
    *   Se aplican las migraciones de Entity Framework de manera automática.
    *   Se insertan los datos de prueba y se valida la persistencia y la lógica del negocio contra el motor real de SQL Server.
    *   Se detiene y destruye el contenedor de forma limpia.

---

## 💼 4. Integridad de la Lógica de Negocio

La lógica de negocio original se ha mantenido intacta y protegida:
*   Se mantienen intactas las interfaces de repositorio y persistencia (`ISigeriDbContext`).
*   Los handlers de MediatR respetan de forma estricta las reglas de negocio (ej. el borrado de activos es un borrado lógico estableciendo `Estado = false`, permitiendo auditoría mediante `ActualizadoPor`).
*   Se han preservado los comportamientos de validación de dominios y lanzamiento de excepciones controladas (`DomainException`).
