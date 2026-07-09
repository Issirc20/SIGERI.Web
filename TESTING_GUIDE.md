# 🚀 SIGERI - Testing & Navigation Guide

## URL de Prueba

La aplicación está corriendo en: **http://localhost:5100**

---

## 📍 Rutas de Navegación

### Dashboard Principal
```
GET http://localhost:5100/
```
✅ KPIs, NIST, Tratamientos, Módulos

### Gestión de Activos
```
GET/POST http://localhost:5100/Activos
GET/POST http://localhost:5100/Activos/Create
```
- ✅ Listado con búsqueda instantánea
- ✅ Estadísticas por criticidad
- ✅ Formulario modernizado 4 secciones

### Dependencias Operativas
```
GET/POST http://localhost:5100/Dependencias
GET/POST http://localhost:5100/Dependencias/Create
```
- ✅ Relación Origen → Destino
- ✅ Matriz visual 5x5 criticidad
- ✅ Búsqueda full-text

### Perfiles Críticos
```
GET/POST http://localhost:5100/PerfilesCriticos
GET/POST http://localhost:5100/PerfilesCriticos/Create
```
- ✅ Grid de tarjetas (no tabla)
- ✅ Análisis 5D de impacto
- ✅ Dimensiones: Reputación, Financiero, Productividad, Legal, SSOMA

### Controles de Madurez
```
GET/POST http://localhost:5100/ControlesMadurez
GET/POST http://localhost:5100/ControlesMadurez/Create
```
- ✅ Niveles 0-4 con matriz visual
- ✅ NIST CSF 2.0 / CIS v8
- ✅ Cálculo de brecha automático

---

## 🎬 FLUJO DE PRUEBA RECOMENDADO

### 1️⃣ Vista Índice (Listado)
Entra a cada módulo y verifica:
- [ ] Page header con badge
- [ ] Estadísticas en tiempo real
- [ ] Barra de búsqueda funcional
- [ ] Tabla/Grid moderna con colores
- [ ] Botón de acción (Nuevo registro)
- [ ] Responsive en móvil (F12 → Toggle Device)

**Ruta de prueba:**
```
1. Activos → Buscar, filtro, estadísticas
2. Dependencias → Búsqueda, relaciones
3. Perfiles → Grid de tarjetas
4. Controles → Tabla con niveles
```

### 2️⃣ Crear Nuevo Registro
Haz clic en botón "+ Nuevo" en cada módulo:

#### **Activos/Create** ✅
- Llenar: Código, Nombre, Descripción
- Seleccionar: Tipo (dropdown), Criticidad
- Verificar: Focus effects, validación
- Guardar → Regresa a listado

#### **Dependencias/Create** ✅
- Llenar: Origen, Destino (UUID fijos)
- Seleccionar: Tipo
- **PROBAR MATRIZ**: Click en botones de criticidad (1-5)
  - Verifica que el valor se actualiza
  - Colores gradientes funcionan
- Guardar

#### **Perfiles/Create** ✅
- Seleccionar: Activo
- **PROBAR 5D IMPACTS:**
  - ⭐ Reputación: Selecciona "Medio"
  - 💰 Financiero: Selecciona "Alto"
  - 📈 Productividad: Selecciona "Bajo"
  - ⚖️ Legal: Selecciona "Alto"
  - 👥 SSOMA: Selecciona "Medio"
- Llenar: Contenedores (no obligatorio)
- Guardar

#### **Controles/Create** ✅
- Llenar: Código (ej: NIST-GV.01), Nombre
- Seleccionar: Función (Gobernanza, ID, etc.)
- **PROBAR MATRIZ MADUREZ:**
  - Actual: Click en botón "2️⃣ Repetible"
  - Objetivo: Click en botón "4️⃣ Optimizado"
  - **Verificar Brecha:** Debería mostrar "2 niveles"
  - Progress bar llena al 50%
  - Usar sliders también
- Llenar: Descripción
- Guardar

### 3️⃣ Validaciones
- [ ] Campos obligatorios marcan error
- [ ] Enums muestran opciones correctas
- [ ] Anti-CSRF token funciona
- [ ] Model binding valida tipos

---

## 💾 Datos de Prueba

### UUIDs Predefinidos
```
Organización: 00000000-0000-0000-0000-000000000001
Activo 1 (ACT-001): 00000000-0000-0000-0000-000000000001
Activo 2 (ACT-002): 00000000-0000-0000-0000-000000000002
Activo 3 (ACT-003): 00000000-0000-0000-0000-000000000003
```

### Enums Dinámicos
- **TipoActivo**: Software, Hardware, Datos_Criticos, etc.
- **Criticidad**: Baja, Media, Alta, Crítica
- **TipoDependenciaActivo**: Operativa, Técnica, etc.
- **NivelImpactoOctave**: Bajo, Medio, Alto
- **FuncionNist**: Gobernanza, Identificación, Protección, Detección, Respuesta, Recuperación

---

## 🎨 Características a Verificar

### Estilos
- [ ] Dark mode activo con colores correctos
- [ ] Gradientes suave en botones
- [ ] Sombras elevadas en cards
- [ ] Border radius consistente
- [ ] Animaciones smooth (hover, focus)

### Matrices Visuales
- [ ] **Dependencias**: 5x1 horizontal con emojis
  - 🟢 Baja | 🔵 Media | 🟡 Alta | 🔴 Crítica | ⚫ Máxima
- [ ] **Controles**: 5x1 horizontal con números
  - 0️⃣ No | 1️⃣ Inicial | 2️⃣ Repetible | 3️⃣ Definido | 4️⃣ Optimizado

### Funcionalidad JavaScript
- [ ] Range sliders actualizan labels
- [ ] Click en matriz actualiza input
- [ ] Progress bar se recalcula
- [ ] Gap shows diferencia niveles

---

## 🔄 Hot Reload

Si modificas un archivo:
```
1. Guarda el archivo en VS
2. El navegador se recarga automáticamente
3. Si no, presiona F5 manualmente
```

---

## 📊 Framework & Requisitos

✅ **NIST CSF 2.0** integrado
✅ **6 Funciones NIST** en Controles
✅ **5 Dimensiones de Impacto** en Perfiles
✅ **5x5 Matriz de Criticidad** en Dependencias
✅ **4 Niveles de Madurez** (0-4) en Controles
✅ **Análisis 5D** (Reputación, Financiero, Productividad, Legal, SSOMA)

---

## 🐛 Debugging

Si algo falla:

1. **F12 → Console**: Busca errores JavaScript
2. **F12 → Network**: Verifica respuestas HTTP
3. **Visual Studio**: Revisa Output window del build
4. **Terminal**: Busca errores en dotnet run

Haz clic en siguiente módulo y prueba de nuevo...

---

## ✅ CHECKLIST FINAL

- [x] Compilación exitosa ✅
- [x] 4 Visitas Index modernizadas
- [x] 4 Formularios Create mejorados
- [x] Matriz de criticidad (Dependencias)
- [x] Matriz de madurez (Controles)
- [x] Análisis 5D (Perfiles)
- [x] Estadísticas en tiempo real
- [x] Búsqueda instantánea
- [x] Responsive mobile-first
- [x] Dark/Light mode
- [x] Animaciones suaves
- [x] Estilos profesionales
- [x] Hot reload funcional
- [x] Documentation completa

---

**¡Listo para probar en el navegador!** 🚀
