# Plan de Implementación - Rediseño del Dashboard & Integración de Pruebas

Este documento detalla el plan técnico aprobado para la modernización de la plataforma **SIGERI**, centrado en la interfaz ejecutiva del Dashboard, la corrección del script de verificación local, la cobertura del 90%+ de pruebas unitarias y la integración de pruebas E2E con Testcontainers.

---

## 🎯 1. Objetivos del Proyecto
1.  **Rediseño de la Interfaz del Dashboard**: Convertir la UI original en una consola ejecutiva moderna de ciberseguridad industrial responsiva (móvil, tablet y escritorio) basada en una grilla CSS de 12 columnas y espaciado de grilla de 8px.
2.  **Dashboard Modularizado**: Consolidar todos los widgets (kpis, gráficos interactivos, matriz de riesgo 5x5, timeline y centro de notificaciones) en una grilla responsive eliminando sidebars laterales innecesarios.
3.  **Estabilización del Script de Validación**: Adaptar el script `Validate-SIGERI.ps1` para que sea compatible con PowerShell 5.1 (entornos Windows corporativos), incorporando un flujo de inicio de sesión programático con cookies de autenticación y tokens anti-forgery para pruebas de humo reales.
4.  **Cobertura de Pruebas Unitarias al 90%+**: Cubrir todos los manejadores de comandos/consultas y validadores en la capa de aplicación (`SIGERI.Application`), logrando 100% en `SIGERI.Domain` y excluyendo ensamblados externos.
5.  **Infraestructura de Pruebas de Integración**: Configurar pruebas de integración que se ejecuten contra un motor real de MS SQL Server en Docker usando **Testcontainers**.

---

## 🛠️ 2. Cambios Propuestos por Componente

### Capa de Presentación (`SIGERI.Web`)
*   **[Index.cshtml](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web/Views/Home/Index.cshtml)**: Reconstrucción total con grilla de 12 columnas.
*   **[_Layout.cshtml](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web/Views/Shared/_Layout.cshtml)**: Corrección de directivas CSS escapando `@media` a `@@media`. Adición del wrapper principal con topbar FontAwesome unificado y breadcrumbs globales.
*   **[dashboard.css](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web/wwwroot/css/dashboard.css)**: Nueva hoja de estilo externa para grillas, tarjetas y animaciones.
*   **[dashboard.js](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Web/wwwroot/js/dashboard.js)**: Script externo para inicialización de gráficos (Chart.js), interacción dinámica de la matriz de riesgo 5x5, y consumo de datos JSON.

### Capa de Pruebas Unitarias (`SIGERI.UnitTests`)
*   **[DependencyInjection.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.Application/DependencyInjection.cs)**: Registrar `[assembly: InternalsVisibleTo("SIGERI.UnitTests")]` para permitir la instanciación de manejadores internos.
*   **[ApplicationRemainingCoverageTests.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.UnitTests/Application/ApplicationRemainingCoverageTests.cs)**: Nueva suite que cubre el 100% de la lógica pendiente (casos de uso de activos, usuarios, evaluaciones, riesgos, tratamientos, validadores FluentValidation y pipeline de validación).

### Capa de Pruebas de Integración (`SIGERI.IntegrationTests`)
*   **[SIGERI.IntegrationTests.csproj](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.IntegrationTests/SIGERI.IntegrationTests.csproj)**: Nuevo proyecto integrado a la solución con dependencias de `Testcontainers.MsSql`, `Microsoft.EntityFrameworkCore.SqlServer` y `FluentAssertions`.
*   **[SigeriIntegrationTests.cs](file:///c:/Users/USUARIO/source/repos/SIGERI.Web/SIGERI.IntegrationTests/SigeriIntegrationTests.cs)**: Pruebas con levantamiento de MS SQL Server en Docker, migración automática del esquema relacional y aserción de transacciones de registro y borrado lógico contra la base de datos real.

---

## 🧪 3. Plan de Verificación y Criterios de Aceptación
1.  **Compilación**: La solución completa debe compilar sin advertencias ni errores en modo Debug/Release.
2.  **Pruebas Unitarias**: Paso exitoso de las 59 pruebas unitarias.
3.  **Pruebas de Integración**: Ejecución exitosa de las pruebas de integración contra SQL Server en Docker de manera local.
4.  **Validación de Script**: El script `Validate-SIGERI.ps1` debe finalizar con código de salida `0` y calificar el portal con estado **"EXCELENTE"** en el reporte del Quality Gate.
