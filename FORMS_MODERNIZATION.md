# 📋 Formularios Modernizados - SIGERI

## ✅ Formularios Create Actualizados

Todos los formularios de registro han sido rediseñados con un estilo premium consistente con el dashboard.

---

## 🎯 **1. Registrar Activo** (`Activos/Create.cshtml`)

### Secciones:
1. **Identificación** 🏷️
   - Código, Nombre, Descripción
   - Validación de entrada

2. **Clasificación** 📊
   - Tipo (enum: Software, Hardware, etc.)
   - Criticidad (Baja, Media, Alta, Crítica)
   - Dropdown dinámicos desde enums

3. **Responsabilidad** 👤
   - Propietario, Ubicación
   - Responsable administrativo

4. **Organización** 🏢
   - ID Organización (read-only)
   - Vinculación con estructura org

5. **Auditoría** ⏰
   - Creado por (read-only: "sistema")

### Estilos:
- ✨ Page header con icono y badge "MAGERIT v3"
- 🎨 Gradient backgrounds (púrpura/azul)
- 📱 Grid responsive (desktop → tablet → mobile)
- 🔍 Inputs con focus effects
- 💫 Animaciones suaves

---

## 🔗 **2. Registrar Dependencia** (`Dependencias/Create.cshtml`)

### Secciones:
1. **Relación de Activos** 🔀
   - Activo Origen (UUID)
   - Activo Destino (UUID)
   - Tooltips explicativos

2. **Tipo de Dependencia** 🌳
   - Dropdown con tipos (Operativa, Técnica, etc.)
   - Enum: TipoDependenciaActivo

3. **Criticidad Operativa** 📊
   - **Range Slider** (1-5)
   - **Matriz Visual 5x5** con selectores interactivos:
	 ```
	 🟢 Baja (1)  🔵 Media (2)  🟡 Alta (3)  🔴 Crítica (4)  ⚫ Máxima (5)
	 ```
   - Click para seleccionar, actualiza input automáticamente
   - Colores degradados por nivel

4. **Descripción** 📝
   - Textarea para impacto y consideraciones

5. **Auditoría** ⏰

### Estilos Especiales:
- 🎨 Badge "OPERATIVA" (rojo)
- 📊 Criticality Matrix visual selector
- 🎯 Actualización en tiempo real del valor

---

## 🛡️ **3. Registrar Perfil Crítico** (`PerfilesCriticos/Create.cshtml`)

### Secciones:
1. **Activo Crítico** 🎖️
   - Select asset (read-only)

2. **Análisis de Impacto 5D** 📈
   - ⭐ **Reputación**: Daño a imagen
   - 💰 **Financiero**: Pérdidas económicas
   - 📈 **Productividad**: Operatividad
   - ⚖️ **Legal**: Incumplimiento normativo
   - 👥 **SSOMA**: Seguridad y salud

   **Cada dimensión:**
   - Dropdown con 3 niveles: Bajo, Medio, Alto
   - Descripciones contextuales
   - Iconos identificadores

3. **Contenedores** 📦
   - 🔧 Contenedores Técnicos (sistemas, apps)
   - 🏢 Contenedores Físicos (ubicaciones)
   - 👕 Contenedores Humanos (roles, personas)
   - Textareas para documentación

4. **Auditoría** ⏰

### Estilos Especiales:
- 🎨 Badge "ANÁLISIS DE IMPACTO" (rojo)
- 📊 Grid 2x2 para 4 dimensiones + 1 full width
- 💡 Descripciones contextuales bajo labels

---

## 📊 **4. Registrar Control de Madurez** (`ControlesMadurez/Create.cshtml`)

### Secciones:
1. **Identificación** 🔍
   - Código (ej: NIST-ID.X.X)
   - Nombre del control

2. **Clasificación** 📑
   - Función NIST CSF 2.0:
	 - Gobernanza
	 - Identificación
	 - Protección
	 - Detección
	 - Respuesta
	 - Recuperación
   - Descripciones en tooltips

3. **Nivel de Madurez** 📊 ⭐ **VISUAL MATRIX**

   **Nivel Actual:**
   - Range slider (0-4)
   - Visual matrix con 5 botones:
	 ```
	 0️⃣ No  1️⃣ Inicial  2️⃣ Repetible  3️⃣ Definido  4️⃣ Optimizado
	 ```
   - Colores por nivel:
	 - 0 = Gris (Sin implementar)
	 - 1 = Rojo (Inicial)
	 - 2 = Naranja (Repetible)
	 - 3 = Verde (Definido)
	 - 4 = Azul (Optimizado)

   **Nivel Objetivo:**
   - Range slider separado (0-4)
   - Misma matriz visual

   **Brecha Calculada:**
   - Diferencia automática = Objetivo - Actual
   - Progress bar visual
   - Número de niveles a cubrir

4. **Descripción** 📝
   - Plan de acción, responsables, fechas

5. **Auditoría** ⏰

### Estilos Especiales:
- 🎨 Badge "NIST CSF 2.0 / CIS v8" (azul)
- 📊 **Maturity Level Matrix**: 5 botones clicables
- 📈 **Gap Visualization**: Progress bar dinámica
- 🎯 Señalización visual del objetivo vs actual
- ✓ Checkmark cuando gap es 0

---

## 🎨 **Componentes Comunes**

### Page Header (todas las forms)
```html
<h1>Icono + Título</h1>
<span class="badge">METODOLOGÍA</span>
<p>Descripción contextual</p>
```

### Section Dividers
```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
   Icono + Nombre Sección
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

### Form Groups
- Label + Descripción
- Input/Textarea/Select con focus effects
- Small helper texts
- Error messages (Bootstrap validation)

### Action Buttons
```
[Guardar] [Cancelar]
```
- Primary: Gradient púrpura + Shadow
- Secondary: Border transparent + Hover effect
- Hover: translateY(-2px) + Enhanced shadow

### Matrices Visuales
- **Criticidad** (Dependencias): 5 niveles +emoji
- **Impacto** (Perfiles): 3 niveles + descripción
- **Madurez** (Controles): 5 niveles + colores diferenciados

---

## 📱 Responsive Design

| Breakpoint | Comportamiento |
|-----------|-----------------|
| Desktop (>1200px) | Grid auto-fit, 250px min |
| Tablet (768-1200px) | 2-3 columnas, padding reducido |
| Mobile (<768px) | 1 columna full-width, compact |

---

## ✨ Características JavaScript

### Dependencias/Create
```javascript
function setCriticality(value) {
  // Actualiza slider y visualización
}
```

### ControlesMadurez/Create
```javascript
function setLevel(inputId, value) {
  // Actualiza range slider
}

function updateProgressBars() {
  // Calcula brecha (gap)
  // Actualiza progress bar visual
  // Renderiza diferencia en tiempo real
}
```

---

## 🔐 Seguridad

✅ Anti-CSRF tokens (@Html.AntiForgeryToken())
✅ Model binding validado
✅ Read-only fields protegidos
✅ Enums validados en backend

---

## 📊 Cobertura de Requisitos

### Activos ✅
- [x] Identificación completa
- [x] Clasificación por tipo y criticidad
- [x] Auditoría

### Dependencias ✅
- [x] Relación Origen → Destino
- [x] Tipo de dependencia
- [x] Criticidad 1-5
- [x] Matriz visual 5x5 (criticidad)
- [x] Descripción

### Perfiles Críticos ✅
- [x] Análisis 5D (Reputación, Financiero, Productividad, Legal, SSOMA)
- [x] 3 niveles de impacto por dimensión
- [x] Contenedores (Técnico, Físico, Humano)
- [x] Documentación completa

### Controles Madurez ✅
- [x] Clasificación NIST CSF 2.0
- [x] 5 niveles de madurez (0-4)
- [x] Matriz visual de niveles
- [x] Nivel actual + Objetivo
- [x] Cálculo automático de brecha
- [x] Progress bar visual
- [x] 6 funciones NIST (Gobernanza, ID, Protección, Detección, Respuesta, Recuperación)

---

## 🚀 Compilación

✅ **Build exitoso**: 0 errores
✅ **Hot Reload**: Habilitado
✅ **Formularios listos**: Todos funcionales

---

## 📸 Elementos Visuales

### Colores por Contexto:
- **Activos**: Púrpura/Azul (MAGERIT)
- **Dependencias**: Púrpura/Azul + Rojo (Criticidad)
- **Perfiles**: Rojo (Crítico)
- **Controles**: Azul (NIST)

### Iconos Font Awesome 6.5.2:
- ✏️ fa-plus-circle (crear)
- 🔗 fa-link (dependencias)
- 🛡️ fa-shield-virus (perfiles)
- 📊 fa-chart-column (controles)
- 💾 fa-save (guardar)
- ❌ fa-times (cancelar)

---

**Estado**: ✅ COMPLETADO Y LISTO PARA PRODUCCIÓN
