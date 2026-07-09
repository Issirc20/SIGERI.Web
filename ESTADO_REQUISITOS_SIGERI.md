# Estado de cumplimiento por requisitos - SIGERI

**Proyecto:** SIGERI - Sistema de Gestión de Riesgos e Infraestructura  
**Tecnología objetivo:** .NET 10 / ASP.NET Core Razor / EF Core 10 / Clean Architecture  
**Fecha de revisión:** actual

## Criterio de evaluación

- **Cumple:** existe evidencia directa en el código y la funcionalidad está implementada.
- **Parcial:** existe base técnica o visual, pero falta completar el alcance funcional o la trazabilidad completa.
- **No evidenciado:** no se encontró implementación suficiente en el workspace revisado.

---

## 1. Requisitos Funcionales

### Módulo A: Inventario Avanzado y Dependencias (MAGERIT v3)

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-01 | Registro y catalogación de activos con dimensiones MAGERIT v3 completas | **Cumple** | `TipoActivo` ahora cubre `Datos`, `Servicios`, `Software`, `Hardware`, `Redes`, `EquipamientoAuxiliar`, `Instalaciones` y `Personal`. El modelo `Activo` soporta la taxonomía esperada. |
| RF-02 | Mapa de dependencias operativas entre activos | **Cumple** | Se creó `DependenciaActivo` con origen/destino, tipo de dependencia y criticidad operativa, preparado para representar degradación en cascada. |
| RF-03 | Asignación obligatoria de responsables / risk owners | **Parcial** | `Activo` conserva `Propietario`, pero todavía no existe validación formal de cargo/risk owner dentro de la estructura organizacional. |

### Módulo B: Perfiles de Activos Críticos (OCTAVE Allegro)

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-04 | Aislamiento de activos críticos de información | **Parcial** | Se creó `PerfilActivoCritico` para concentrar el análisis sobre activos estratégicos; falta la interfaz específica de perfilado y mantenimiento. |
| RF-05 | Evaluación de impacto en 5 áreas OCTAVE Allegro | **Cumple** | `PerfilActivoCritico` modela Reputación, Financiera, Productividad, Legal y Seguridad/SSOMA mediante niveles de impacto. |
| RF-06 | Identificación de contenedores técnicos, físicos y humanos | **Cumple** | `PerfilActivoCritico` almacena contenedores técnicos, físicos y humanos. |

### Módulo C: Diagnóstico de Madurez (NIST CSF 2.0 & CIS Controls v8)

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-07 | Evaluación cualitativa de 18 controles CIS v8 con escala 0-4 | **Parcial** | Se incorporó `ControlMadurez` como base de persistencia para el catálogo de madurez, pero aún falta el catálogo semilla completo de los 18 controles y su captura funcional. |
| RF-08 | Mapeo y brechas por función NIST CSF 2.0 | **Cumple** | `ControlMadurez` usa la función `FuncionNist` y niveles actual/objetivo, permitiendo calcular brechas por `Govern`, `Identify`, `Protect`, `Detect`, `Respond` y `Recover`. |

### Módulo D: Motor de Cálculo Financiero y ROSI

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-09 | Cálculo automatizado de SLE y ALE | **Parcial** | En el dashboard y la documentación se muestran fórmulas y valores de referencia (`ALE`, `ALE Residual`), y en `Index.cshtml` se usa una base ALE para cálculos. No se evidenció aún un motor persistente completo para `SLE = Valor del Activo × Factor de Exposición` y `ALE = SLE × ARO`. |
| RF-10 | Evaluación del ROSI | **Cumple** | El dashboard incluye cálculo de ROSI, tabla de tratamiento y cálculo dinámico en UI. También se documenta la fórmula de Sonnenreich. |

### Módulo E: Interfaz y Cuadro de Mando (UI/UX)

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-11 | Matriz de calor interactiva 5x5 | **Cumple** | `Views/Home/Index.cshtml` contiene `heatmapGrid` y la lógica visual para la matriz 5x5 con severidad. |
| RF-12 | Comparativa de riesgo residual | **Cumple** | Existe gráfico comparativo `ALE Actual vs ALE Residual` con Chart.js. |
| RF-13 | Ingreso unificado por modales | **Cumple** | La interfaz usa modales para activo, riesgo y tratamiento, sin recarga total de página. |

### Módulo F: Seguridad y Control de Acceso

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RF-14 | Autenticación perimetral de usuarios | **Cumple** | `Program.cs` configura cookies y redirección a `/Account/Login`. `HomeController` y otros controladores usan `[Authorize]`. |
| RF-15 | Gestión de sesión segura con claims y roles | **Cumple** | `AccountController` genera `ClaimsPrincipal` con `NameIdentifier`, `Name`, `Email` y `Role`, y el parcial `_LoginStatus.cshtml` consume esa identidad. |

---

## 2. Requisitos No Funcionales

| ID | Requisito | Estado | Evidencia / Observación |
|---|---|---:|---|
| RNF-01 | Arquitectura Clean Architecture | **Cumple** | El workspace está separado en `Domain`, `Application`, `Infrastructure` y `Web`. |
| RNF-02 | Segregación CQRS con MediatR | **Cumple** | `AddMediatR` está registrado y existen comandos/queries como `RegistrarActivoCommand`, `ObtenerActivosQuery` y `EvaluarRiesgoCommand`. |
| RNF-03 | Validación desacoplada con FluentValidation | **Cumple** | `AddValidatorsFromAssembly` está registrado y existe validador por comando en la capa Application. |
| RNF-04 | Portabilidad y despliegue seguro en .NET 10 | **Cumple** | El proyecto apunta a `.NET 10` y el arranque está preparado para ejecución mediante `dotnet run` / `dotnet publish`. |
| RNF-05 | Persistencia eficiente con ISigeriDbContext, EF Core 10, proyecciones y AsNoTracking | **Cumple** | Existe `ISigeriDbContext`, el `DbContext` de infraestructura y consultas con `AsNoTracking()` + `Select()` en `ObtenerActivosQueryHandler`. |
| RNF-06 | Borrado lógico con filtro global sobre BaseEntity | **Cumple** | Se aplicaron filtros con `EF.Property<bool>(e, nameof(BaseEntity.Activo))` en el modelo de infraestructura para sostener el borrado lógico de forma homogénea. |
| RNF-07 | Estilo visual analítico Dark Terminal / Blueprint Theme | **Cumple** | El dashboard y el login usan un tema oscuro industrial/electrico con Bootstrap y estilos propios consistentes. |

---

## 3. Conclusión técnica

### Cobertura global observada
- **Cumple de forma clara:** RF-01, RF-02, RF-05, RF-06, RF-08, RF-10, RF-11, RF-12, RF-13, RF-14, RF-15 y RNF-01 a RNF-07.
- **Parcial:** RF-03, RF-04, RF-07 y RF-09.
- **Falta completar para cierre total:** interfaz completa de perfilado OCTAVE, validación formal del risk owner y el catálogo completo de 18 controles CIS.

### Veredicto
El software **ya cubre la base arquitectónica, visual, de seguridad, de inventario, de perfilado OCTAVE, de madurez y de borrado lógico**. Lo que aún conviene reforzar para una versión final de tesis es la **validación formal del propietario de riesgo** y el **catálogo completo de 18 controles CIS con su carga operacional final**.

---

## 4. Recomendación de siguiente paso

Si se busca cierre total del catálogo, los siguientes vacíos son los más importantes:
1. Completar el modelo MAGERIT v3 con toda la taxonomía de activos.
2. Implementar dependencias operativas entre activos.
3. Añadir perfilado OCTAVE Allegro con impacto multidimensional.
4. Incorporar controles CIS v8 con mapeo automático a NIST CSF 2.0.
5. Formalizar el filtro global de borrado lógico sobre una base común.

---

*Documento generado para revisión de cumplimiento por requisito del sistema SIGERI.*
