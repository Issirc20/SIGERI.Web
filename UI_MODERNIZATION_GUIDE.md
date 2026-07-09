# Guía de Componentes UI Modernos - SIGERI Dashboard

## 📋 Descripción General

Se ha implementado un **layout completamente modernizado** para la aplicación SIGERI con:

✅ **Sidebar navegable y colapsable** con transiciones suaves  
✅ **Dark/Light Mode Toggle** persistente en localStorage  
✅ **Componentes reutilizables** para tarjetas, botones y métricas  
✅ **Grid responsivo mobile-first** para todas las secciones  
✅ **Tratamientos en cards** en lugar de tabla tradicional  
✅ **Animaciones modernas** con Cubic Bezier  
✅ **Variables CSS personalizadas** para fácil personalización  

---

## 🎨 Nuevo Layout (`_Layout.cshtml`)

### Características Principales

1. **Sidebar Fijo (Izquierda)**
   - Ancho: 280px (expandido) / 80px (colapsado)
   - Logo con icono SIGERI
   - Navegación con 5 módulos + menú secundario
   - Transición suave de colapso

2. **Navbar Superior**
   - Botón toggle sidebar (hamburguesa)
   - Mensaje de bienvenida (oculto en mobile)
   - **Dark/Light Mode Toggle** en tiempo real
   - Área de usuario con `_LoginStatus`

3. **Main Content**
   - Margen izquierdo dinámico según sidebar state
   - Padding responsive (2rem en desktop, 1rem en mobile)
   - Overflow-y para scroll independiente

4. **Footer**
   - Margen dinámico ligado al sidebar
   - Información centrada y compacta

### Variantes de Color (CSS Variables)

```css
:root {
	--sig-dark-bg: #070a0f;           /* Fondo principal */
	--sig-dark-surface: #0f1319;      /* Cards, navbar */
	--sig-dark-surface-2: #0c0f15;    /* Gradiente secundario */
	--sig-dark-border: rgba(255, 255, 255, 0.07);
	--sig-dark-text: #f0f4f9;
	--sig-dark-muted: #7a8895;
	--sig-accent: #5d3fa0;            /* Púrpura principal */
	--sig-accent-hover: #734ab3;      /* Púrpura hover */
	--sig-accent-2: #8b5cf6;          /* Púrpura claro */
	--sig-blue: #3b82f6;
}

/* Light mode automático con prefers-color-scheme */
@media (prefers-color-scheme: light) {
	:root {
		--sig-dark-bg: #f9fafb;
		--sig-dark-surface: #ffffff;
		--sig-dark-surface-2: #f3f4f6;
		/* ... */
	}
}
```

---

## 🃏 Dashboard Redesignado (`Home/Index.cshtml`)

### Estructura por Secciones

#### 1. **KPI Cards Grid** (4 columnas responsivas)
```html
- ALE Expuesto: riesgo sin mitigación
- Inversión Plan: costo total de controles
- ALE Residual: riesgo después de mitigation
- ROSI %: retorno sobre inversión en seguridad
```

Cada card incluye:
- Etiqueta descriptiva (uppercase)
- Valor grande monoespaciado
- Icono con color temático
- Hover: elevación + shadow

#### 2. **NIST Functions Grid** (6 funciones)
- Nombre de función
- Barra de progreso visual
- Puntuación actual / meta (4.0)
- Brecha calculada
- % completado

#### 3. **Plan de Tratamiento (Grid Cards)**
⚠️ **CAMBIO IMPORTANTE**: Reemplazó tabla tradicional por **grid de tarjetas**

Cada tarjeta contiene:
- **ID Riesgo**: badge con gradiente
- **Criticidad**: color variable (Crítica/Alta/Media/Baja)
- **Escenario**: nombre del riesgo
- **Control**: salvaguarda propuesta (condensada)
- **Métricas en grid 2x2**:
  - Inversión | Mitigación %
  - ALE Residual | ROSI %
- **Responsable**: footer con propietario

Colores por criticidad:
```css
.sig-badge-critical  /* Rojo: >= 300K */
.sig-badge-high      /* Amarillo: >= 150K */
.sig-badge-medium    /* Naranja: >= 80K */
.sig-badge-low       /* Verde: < 80K */
```

#### 4. **Admin Modules Grid** (4 accesos rápidos)
Enlaces a:
- Inventario de Activos (info)
- Dependencias (advertencia)
- Perfiles Críticos (éxito)
- Madurez CIS/NIST (peligro)

Hover effect: elevación + zoom de icono

---

## 🔧 Componentes Reutilizables

### `_MetricCard.cshtml`
Tarjeta de métrica/KPI reutilizable.

**Modelo:**
```csharp
(string Label, string Value, string Icon, string AccentColor)
```

**Uso:**
```html
@await Html.PartialAsync("_MetricCard", ("ALE Expuesto", "S/ 870K", "fa-exclamation-triangle", "220, 53, 69"))
```

---

### `_ButtonComponent.cshtml`
Botón moderno con icono.

**Modelo:**
```csharp
(string Text, string Icon, string Url, string Class)
```

**Uso:**
```html
@await Html.PartialAsync("_ButtonComponent", ("Nuevo Activo", "fa-plus", Url.Action("Create", "Activos"), ""))
```

---

### `_InfoCard.cshtml`
Tarjeta de información con icono y enlace.

**Modelo:**
```csharp
(string Title, string Description, string Icon, string IconColor, string Link)
```

**Uso:**
```html
@await Html.PartialAsync("_InfoCard", ("Inventario", "Gestiona activos MAGERIT", "fa-server", "#3b82f6", Url.Action("Index", "Activos")))
```

---

### `_NistCard.cshtml`
Tarjeta de función NIST con barra de progreso.

**Modelo:**
```csharp
(string Name, decimal Score, decimal Target)
```

**Uso:**
```html
@await Html.PartialAsync("_NistCard", ("Govern", 2.8m, 4m))
```

---

## 📱 Responsive Design

### Breakpoints

| Tamaño | Cambios |
|--------|---------|
| **Desktop** (769px+) | Sidebar 280px, KPI grid 4 col, Treatments 3 col |
| **Tablet** (481-768px) | Sidebar 80px colapsado, KPI grid 2 col, Treatments 1 col |
| **Mobile** (<480px) | Sidebar colapsado, todo 1 columna, padding reducido |

### Grid Template
```css
/* KPI Cards */
grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));

/* Treatments - Perfecto para cards */
grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));

/* NIST Functions */
grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));

/* Admin Modules */
grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
```

---

## 🌓 Dark/Light Mode

### Funcionamiento

1. **Detección automática**: Sistema operativo preferences
2. **Toggle manual**: Botones en navbar (oscuro/claro)
3. **Persistencia**: localStorage `theme` (dark/light)

### JavaScript
```javascript
// Guardado automático en localStorage
localStorage.setItem('theme', isDark ? 'dark' : 'light');

// Restauración al cargar
const savedTheme = localStorage.getItem('theme');
if (savedTheme) setTheme(savedTheme === 'dark');
```

### Implementación en CSS
```css
@media (prefers-color-scheme: light) {
	:root {
		/* Variables claras */
	}
}
```

---

## 🚀 Mejoras Implementadas

### Antes vs Después

| Aspecto | Antes | Después |
|--------|-------|---------|
| **Navegación** | Top navbar solo | Sidebar + navbar |
| **Treatments** | Tabla HTML grande | Cards grid responsivo |
| **Densidad** | Vacíos grandes | Contenido compacto |
| **Tema** | Solo oscuro | Dark + Light toggle |
| **Componentes** | Inline en Index | 4 partials reutilizables |
| **Animaciones** | Básicas | Cubic bezier, hover effects |
| **Mobile** | Colapsada | Completamente responsive |

---

## 📌 Variables Clave de Personalización

En `_Layout.cshtml`:

```css
:root {
	--sig-dark-bg: #070a0f;              /* Cambiar fondo */
	--sig-accent: #5d3fa0;               /* Cambiar color primario */
	--sig-accent-2: #8b5cf6;             /* Cambiar acentos */
	--sig-sidebar-width: 280px;          /* Ancho sidebar */
	--sig-sidebar-collapsed: 80px;       /* Ancho colapsado */
}
```

En `Home/Index.cshtml`:

```css
:root {
	--sig-spacing: 1.5rem;               /* Gap entre grids */
	--sig-radius: 1rem;                  /* Border radius cards */
	--sig-duration: 0.25s;               /* Duración animaciones */
}
```

---

## 🔄 Rutas de Navegación

- `Dashboard` → Home/Index
- `Activos` → Activos/Index
- `Dependencias` → Dependencias/Index
- `Perfiles` → PerfilesCriticos/Index
- `Madurez` → ControlesMadurez/Index
- `Configuración` → # (placeholder)
- `Ayuda` → # (placeholder)

---

## ✨ Próximas Mejoras (Opcionales)

- [ ] Agregar breadcrumbs dinámicos
- [ ] Implementar filtros en tratamientos
- [ ] Exportar reportes (PDF/Excel)
- [ ] Gráficos interactivos con Chart.js
- [ ] Búsqueda global en navbar
- [ ] Notificaciones en tiempo real
- [ ] Mapa de calor interactivo
- [ ] Avatares de usuario personalizados

---

## 📞 Soporte

Para personalizar los componentes o agregar nuevas secciones, mantén la estructura de:

1. **CSS clases prefijadas con `sig-`** para evitar conflictos
2. **Variables CSS** para colores y espaciados
3. **Grid auto-fit/auto-fill** para responsiveness
4. **Transiciones cubic-bezier** para suavidad

¡El dashboard está listo para producción! 🚀
