# Backlog de Tareas del Proyecto (task.md)

Este checklist detalla el estado actual de las tareas operativas ejecutadas durante el ciclo de vida del desarrollo guiado por especificaciones (SDD) para el proyecto SIGERI.

---

## 🎨 1. Tareas de Frontend & Rediseño de Dashboard
- [x] Diseñar el sistema de grillas responsivas (12 columnas CSS Grid) en `dashboard.css`.
- [x] Crear e integrar las 6 tarjetas KPI en la parte superior del Dashboard (`_KpiCard.cshtml`).
- [x] Implementar la Matriz de Riesgo 5x5 interactiva para ciberseguridad industrial (`dashboard.js`).
- [x] Configurar gráficos interactivos (Radar de NIST CSF, Cumplimiento ISO 27001, Tendencia de Riesgos) consumiendo datos JSON de los servicios reales.
- [x] Unificar la barra de navegación superior (Topbar) y breadcrumbs dinámicos globales en `_Layout.cshtml`.
- [x] Corregir la clausura de etiquetas CSS en vistas secundarias como `Activos/Index.cshtml`.
- [x] Eliminar los estilos y scripts inline moviéndolos a hojas y scripts externos cargados por secciones.

## 🧪 2. Tareas de Calidad de Código & Pruebas Unitarias
- [x] Exponer ensamblados internos para pruebas agregando `InternalsVisibleTo` en `DependencyInjection.cs` de SIGERI.Application.
- [x] Escribir la suite `ApplicationRemainingCoverageTests.cs` cubriendo comandos y consultas de Evaluaciones, Riesgos, Tratamientos y Usuarios.
- [x] Implementar pruebas unitarias de validación sobre `ValidationBehavior`.
- [x] Agregar pruebas unitarias específicas para todos los validadores FluentValidation.
- [x] Configurar exclusiones de cobertura a nivel de ensamblado en `SIGERI.Web` e `SIGERI.Infrastructure` para asegurar el cumplimiento del 90% mínimo en el core.

## 🐳 3. Tareas de Pruebas de Integración (Testcontainers)
- [x] Instalar y configurar el cliente Docker en el entorno local.
- [x] Crear el proyecto `SIGERI.IntegrationTests.csproj` e instalar `Testcontainers.MsSql` y dependencias de Entity Framework.
- [x] Escribir la suite `SigeriIntegrationTests.cs` configurando el ciclo de vida del contenedor SQL Server.
- [x] Implementar aserciones de registro y borrado lógico relacional.
- [x] Enlazar el proyecto de pruebas de integración en el archivo de la solución `.slnx`.

## ⚙️ 4. Tareas de Estabilización & Scripting de DevOps
- [x] Corregir sintaxis incompatible de PowerShell 5.1 en `Validate-SIGERI.ps1`.
- [x] Implementar flujo autenticado automatizado (Tokens Anti-forgery + Cookie de Sesión) en las llamadas de humo HTTP.
- [x] Limpiar archivos temporales en el directorio raíz de la solución.
- [x] Generar el script relacional de la base de datos `database_schema.sql` mediante herramientas EF Core CLI.
