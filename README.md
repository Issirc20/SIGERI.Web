# 🛡️ SIGERI: Sistema de Gestión de Riesgos e Infraestructura

**Estado del Proyecto:** MVP (Minimum Viable Product) - *Tesis de Ingeniería de Sistemas*  
**Tecnología:** .NET 10, ASP.NET Core Razor Pages, Entity Framework Core 10, CQRS, MediatR.  
**Arquitectura:** Clean Architecture.

---

## 📑 1. Objetivo General
Desarrollar e implementar un software analítico que automatice y converja metodologías internacionales de gestión de riesgos tecnológicos e industriales. SIGERI transforma vulnerabilidades técnicas y operativas en métricas financieras cuantitativas, facilitando la toma de decisiones estratégicas, el cumplimiento normativo (Ley 29733) y la justificación económica de presupuestos de ciberseguridad.

---

## 🔍 2. Fase de Análisis: Convergencia Metodológica
El núcleo científico y de ingeniería de SIGERI radica en la unificación de cuatro marcos de control que estructuran todo el sistema:

1. **MAGERIT v3 (Enfoque Taxonómico / Top-Down):**
   - Clasifica los activos en contenedores (Datos, Servicios, Software, Hardware, Redes, Personal).
   - Establece la trazabilidad de dependencias: Cómo la falla de un equipo auxiliar afecta un proceso core corporativo.
2. **OCTAVE Allegro (Enfoque Selectivo de Negocio):**
   - Filtra los activos críticos del negocio.
   - Mide el impacto en 5 áreas clave: Reputación, Financiero, Productividad, Legal y SSOMA.
3. **NIST CSF 2.0 & CIS Controls v8 (Diagnóstico de Madurez):**
   - Agrupa controles técnicos bajo 6 funciones: *Govern, Identify, Protect, Detect, Respond, Recover*.
   - Diagnostica brechas de madurez (0 a 4) estableciendo la postura de ciberseguridad de la empresa.
4. **Modelado Financiero y ROSI (Expectativa de Pérdida / ROI):**
   - **SLE:** Pérdida Esperada Única.
   - **ARO:** Tasa Anualizada de Ocurrencia.
   - **ALE:** Pérdida Anualizada Esperada ($ALE = SLE \times ARO$).
   - **ROSI:** Retorno de Inversión en Seguridad.

---

## 📐 3. Fase de Diseño: Arquitectura Limpia (Clean Architecture)
El diseño del software sigue principios de separación de responsabilidades para asegurar un ciclo de vida mantenible y agnóstico a integraciones de terceros.

### Capas del Proyecto:
- **SIGERI.Domain (Núcleo / Enterprise Business Rules):**
  - Contiene las entidades puras (Activos, Riesgos, Evaluaciones, Tratamiento).
  - Define Enumeradores (Categorías MAGERIT, Criticidad) y Lógica Inmutable.
- **SIGERI.Application (Casos de Uso / Use Cases):**
  - Implementa el patrón **CQRS** (Command Query Responsibility Segregation) con **MediatR**.
  - Canaliza la lógica transaccional en `Commands` (Registrar, Evaluar, Asignar) y la lectura en `Queries`.
  - Validación perimetral de modelos de entrada usando **FluentValidation**.
- **SIGERI.Infrastructure (Persistencia y Servicios):**
  - Aislamiento de la base de datos vía abstractions (`ISigeriDbContext`).
  - Implementación con **Entity Framework Core 10** (SQL Server).
  - Manejo de borrado lógico interceptado nativamente desde DbContext (`HasQueryFilter`).
- **SIGERI.Web (Presentación / UI Web):**
  - Interfaz gráfica basada en **ASP.NET Core Razor**.
  - Estilos con **Bootstrap 5** (Diseño Dark Terminal / Industrial Blueprint).
  - Visualización interactiva con **Chart.js** y Modales nativos interconectados con APIs.

---

## 🏗️ 4. Modelado de Diseño de Software (SDD - Software Design Document)
La estructura modular (SDD) que rige las especificaciones técnicas del software y su mapa transaccional:

### A. Módulo de Inventario (Gestión de Activos)
- Entidad `Activo`: *Id, Codigo, Nombre, Descripcion, TipoActivo (Enum), Criticidad, Propietario*.
- Flujo: Ingreso guiado por catálogo MAGERIT, asignación de propietario (Dueño del riesgo).

### B. Módulo de Evaluación de Riesgos
- Entidad `EvaluacionRiesgo`: *Calcula y persiste la cruce de Probabilidad (1-5) vs Impacto (1-5).*
- Artefacto visual: **Matriz de Calor Matemática 5x5**. (Zonas Baja, Media, Alta, Crítica).

### C. Módulo de Tratamiento y Analítica Financiera (ROSI)
- Entidad `PlanTratamiento`: *Costos de Salvaguarda, Porcentaje de Mitigación Esperada.*
- Flujo (Motor Matemático): 
  - Resta el porcentaje de mitigación al ALE Base obteniendo el **ALE Residual**.
  - Aplica la fórmula de Sonnenreich para obtener la viabilidad financiera del proyecto (% ROSI).

### D. Sistema de Control de Acceso y Seguridad
- Patrón Perimetral: **Cookies Authentication Middleware**.
- Uso de `ClaimsPrincipal` con asignación de Roles (`Administrador`, `Analista`).
- Atributos `[Authorize]` a nivel de controladores de Razor.

---

## 📊 5. Panel de Rendimiento (Caso de Estudio - Data Semilla)
El MVP precarga métricas base derivadas de incidentes comprobados en infraestructura crítica:
- **ALE Total Expuesto:** S/ 870,000 (Riesgo inherente de Ransomware SCADA, Phishing, Manipulación PLC).
- **Inversión Planificada:** S/ 300,000.
- **ALE Residual Mitigado:** S/ 183,000.
- **ROSI Promedio:** 129% *(Justificativo ante Junta Directiva: Cada 1 S/ invertido ahorra 1.29 S/ adicionales)*.

---

## 🚀 6. Guía de Ejecución (Runbook)

1. Clonar el repositorio del workspace de SIGERI.
2. Abrir terminal en la raíz (`C:\Users\USUARIO\source\repos\SIGERI.Web\`).
3. Restaurar dependencias:  
   ```powershell
   dotnet restore
   ```
4. Ejecutar el proyecto Web:  
   ```powershell
   cd SIGERI.Web
   dotnet run --launch-profile "http"
   ```
5. Acceder en el navegador a `http://localhost:5100`.
6. Credenciales de sustentación habilitadas en entorno local:
   - Se configuran mediante `DevCredentials` en `appsettings.Development.json` o variables de entorno.
   - El usuario administrador y el analista de riesgos se crean automáticamente al iniciar la aplicación en desarrollo.

---
*Este documento SDD (Software Design Document) forma parte de los anexos del proyecto de tesis. Refleja fielmente los procesos de ingeniería de software implementados en el artefacto final.*
