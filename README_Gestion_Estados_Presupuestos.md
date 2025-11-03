# Sistema de Estados y Modos de Presupuestos

## Documentación de la Lógica de Flujo de Estados

Este documento describe la implementación del sistema de gestión de estados de presupuestos y sus diferentes modos de acceso.

---

## ?? Estados del Presupuesto

El sistema maneja 5 estados diferentes para un presupuesto:

| Estado | Valor | Descripción | Permite Edición | Siguiente Estado Posible |
|--------|-------|-------------|-----------------|-------------------------|
| **Borrador** | 0 | Presupuesto en creación/edición | ? Sí | Emitido |
| **Emitido** | 1 | Presupuesto enviado para aprobación | ? No | Aprobado / Rechazado |
| **Aprobado** | 2 | Presupuesto aprobado definitivamente | ? No | *Terminal* |
| **Rechazado** | 3 | Presupuesto rechazado | ? No | *Puede copiarse* |
| **Vencido** | 4 | Presupuesto expirado por fecha | ? No | *Puede copiarse* |

### Reglas de Transición de Estados

```
????????????
? Borrador ? ??? Creación inicial
????????????
     ? Emitir
     ?
????????????
? Emitido  ? ??? Envío para aprobación
????????????
     ?
     ???? Aprobar ? ????????????
     ?              ? Aprobado ? (Estado Final)
     ?              ????????????
     ?
     ???? Rechazar ? ?????????????
                     ? Rechazado ? (Estado Final)
                     ?????????????

????????????
? Vencido  ? ??? Por fecha automática
????????????
```

---

## ?? Modos de Operación del Formulario

El formulario `frmPresupuesto` puede operar en 3 modos diferentes según el acceso desde el menú:

### 1. **Modo GENERAR** (`tsGenerarCotizacion`)

**Propósito:** Crear nuevos presupuestos en estado Borrador.

**Características:**
- ? Solo permite crear nuevos presupuestos
- ? Los presupuestos se crean en estado Borrador (0)
- ? No muestra navegación de presupuestos existentes
- ? No permite modificar/eliminar presupuestos existentes

**Botones Visibles:**
- `btnNuevo` - Crear nuevo presupuesto
- `btnAceptar` - Guardar presupuesto
- `btnCancelar` - Cancelar operación

**Botones Ocultos:**
- Navegación: btnPrimero, btnAnterior, btnSiguiente, btnUltimo, btnBuscar
- Edición: btnModificar, btnEliminar, btnCopiar

**Flujo de Trabajo:**
```
Usuario ? Generar Cotización ? Nuevo formulario vacío ?
Ingresar datos ? Agregar artículos ? Guardar ?
Presupuesto guardado en estado "Borrador"
```

---

### 2. **Modo GESTIONAR** (`tsGestionarCotizacion`)

**Propósito:** Administrar presupuestos existentes (excepto los que están en aprobación).

**Estados Mostrados:**
- ? Borrador (0) - Puede editar/eliminar
- ? Aprobado (2) - Solo lectura
- ? Rechazado (3) - Solo lectura, puede copiar
- ? Vencido (4) - Solo lectura, puede copiar
- ? Emitido (1) - NO se muestra (están en el modo Aprobar)

**Botones Disponibles:**
- **Navegación completa:** btnPrimero, btnAnterior, btnSiguiente, btnUltimo, btnBuscar, btnActualizar
- **Para Borradores:**
  - `btnModificar` - Editar presupuesto
  - `btnEliminar` - Eliminar presupuesto
  - `btnEmitir` ? **NUEVO** - Enviar para aprobación
- **Para cualquier estado:**
  - `btnCopiar` - Crear copia del presupuesto

**Reglas de Negocio:**
```csharp
Estado Borrador (0):
  ? Puede: Editar, Eliminar, Emitir, Copiar
  
Estado Aprobado (2):
  ? Puede: Ver, Copiar
  ? No puede: Editar, Eliminar, Emitir
  
Estado Rechazado (3) o Vencido (4):
  ? Puede: Ver, Copiar
  ? No puede: Editar, Eliminar, Emitir
```

**Botón EMITIR:**
- Solo habilitado para presupuestos en estado Borrador
- Requiere al menos un artículo en el presupuesto
- Al emitir, el presupuesto pasa a estado "Emitido" (1)
- Una vez emitido, desaparece de este modo (va a "Aprobar Cotizaciones")

**Flujo de Trabajo:**
```
Usuario ? Gestión de Cotizaciones ?
Lista de presupuestos (Borrador/Aprobado/Rechazado/Vencido) ?
?? Si es Borrador:
?  ??? Editar ? Emitir ? Sale de la lista
?  ??? Eliminar ? Confirmación ? Eliminado
?? Si es otro estado:
   ??? Solo Ver ? Copiar (opcional)
```

---

### 3. **Modo APROBAR** (`tsAprobarCotizacion`)

**Propósito:** Aprobar o rechazar presupuestos que fueron emitidos.

**Estados Mostrados:**
- ? Emitido (1) - ÚNICAMENTE

**Botones Disponibles:**
- **Navegación:** btnPrimero, btnAnterior, btnSiguiente, btnUltimo, btnBuscar, btnActualizar
- **Aprobación:**
  - `btnAprobarPresupuesto` ? **NUEVO** (Verde) - Aprobar presupuesto
  - `btnRechazarPresupuesto` ? **NUEVO** (Rojo) - Rechazar presupuesto

**Botones Ocultos:**
- btnNuevo, btnModificar, btnEliminar, btnCopiar

**Validaciones:**
- Solo se muestran presupuestos en estado "Emitido" (1)
- Los presupuestos deben tener al menos un artículo
- Confirmación obligatoria antes de aprobar/rechazar
- Muestra resumen del presupuesto en la confirmación

**Flujo de Trabajo:**
```
Usuario ? Aprobar Cotizaciones ?
Lista de presupuestos Emitidos ?
Seleccionar presupuesto ?
??? Aprobar ? Estado = "Aprobado" (2) ? Sale de la lista
??? Rechazar ? Estado = "Rechazado" (3) ? Sale de la lista

Presupuesto Aprobado:
  ? Aparece en "Gestión de Cotizaciones" (solo lectura)
  
Presupuesto Rechazado:
  ? Aparece en "Gestión de Cotizaciones" (solo lectura, puede copiarse)
```

---

## ?? Matriz de Permisos por Modo y Estado

| Acción | Borrador (0) Gestionar | Emitido (1) Aprobar | Aprobado (2) Gestionar | Rechazado (3) Gestionar | Vencido (4) Gestionar |
|--------|----------------------|-------------------|---------------------|---------------------|--------------------|
| **Ver** | ? | ? | ? | ? | ? |
| **Editar** | ? | ? | ? | ? | ? |
| **Eliminar** | ? | ? | ? | ? | ? |
| **Emitir** | ? | ? | ? | ? | ? |
| **Aprobar** | ? | ? | ? | ? | ? |
| **Rechazar** | ? | ? | ? | ? | ? |
| **Copiar** | ? | ? | ? | ? | ? |

---

## ?? Casos de Uso Prácticos

### Caso 1: Creación y Aprobación de Presupuesto

**Vendedor:**
1. Menú ? "Generar Cotización"
2. Completa datos del cliente
3. Agrega productos
4. Guarda (Estado: Borrador)

5. Menú ? "Gestión de Cotizaciones"
6. Busca su presupuesto
7. Revisa y click en "Emitir" (Estado: Emitido)

**Supervisor:**
1. Menú ? "Aprobar Cotizaciones"
2. Ve lista de presupuestos emitidos
3. Revisa detalles
4. Click en "Aprobar" (Estado: Aprobado)

### Caso 2: Presupuesto Rechazado y Nueva Versión

**Vendedor:**
1. Menú ? "Generar Cotización"
2. Crea y emite presupuesto

**Supervisor:**
1. Menú ? "Aprobar Cotizaciones"
2. Revisa y click en "Rechazar" (Estado: Rechazado)

**Vendedor (corrección):**
1. Menú ? "Gestión de Cotizaciones"
2. Encuentra presupuesto rechazado
3. Click en "Copiar"
4. Edita la copia (nuevo presupuesto en Borrador)
5. Guarda y emite nuevamente

### Caso 3: Consulta de Presupuestos Históricos

**Gerente:**
1. Menú ? "Gestión de Cotizaciones"
2. Navega por presupuestos:
   - Ve borradores pendientes
   - Ve presupuestos aprobados (histórico)
   - Ve presupuestos rechazados
   - Ve presupuestos vencidos

---

## ?? Interfaz Visual por Modo

### Modo GENERAR
```
???????????????????????????????????????????????????
? [?Aceptar] [?Cancelar] ? [Nuevo]               ?
???????????????????????????????????????????????????
?                                                 ?
?  [Datos del Cliente]  [Fecha]  [Estado: SOLO  ?
?                                      LECTURA]   ?
?  ???????????????????????????????????????????   ?
?  ?      Artículos (vacío inicialmente)     ?   ?
?  ???????????????????????????????????????????   ?
?  [Totales]                                     ?
???????????????????????????????????????????????????
```

### Modo GESTIONAR
```
???????????????????????????????????????????????????
? [?] [?] ? [Nuevo] [Editar] [Eliminar] [Copiar]?
?          [Emitir*] ? [??] [?] [?] [??] [??] [?]?
???????????????????????????????????????????????????
?  Navegando: Borrador/Aprobado/Rechazado/Vencido?
?  *Emitir solo para Borradores con artículos    ?
???????????????????????????????????????????????????
```

### Modo APROBAR
```
???????????????????????????????????????????????????
? [Aprobar ?] [Rechazar ?] ? [??] [?] [?] [??]  ?
???????????????????????????????????????????????????
?  Mostrando SOLO presupuestos Emitidos (1)      ?
?  Estado: SOLO LECTURA                           ?
???????????????????????????????????????????????????
```

---

## ??? Implementación Técnica

### Archivos Modificados/Creados:

1. **`BLL\Enums\ModoPresupuesto.cs`** ? NUEVO
   - Define los 3 modos de operación

2. **`BLL\Contracts\IPresupuestoService.cs`**
   - Agregado: `GetByEstados(params int[] estados)`

3. **`BLL\Services\PresupuestoService.cs`**
   - Implementado: filtrado por múltiples estados

4. **`SistemaPresupuestario\Presupuesto\frmPresupuesto.cs`**
   - Agregado: `EstablecerModo(ModoPresupuesto modo)`
   - Agregado: `ConfigurarSegunModo()`
   - Modificado: `CargarPresupuestos()` con filtros por modo
   - Modificado: `HabilitarBotones()` con lógica por estado
   - Agregado: `BtnEmitir_Click()` ?
   - Agregado: `BtnAprobarPresupuesto_Click()` ?
   - Agregado: `BtnRechazarPresupuesto_Click()` ?

5. **`SistemaPresupuestario\frmMain.cs`**
   - Agregado: `tsGenerarCotizacion_Click()`
   - Agregado: `tsGestionarCotizacion_Click()`
   - Agregado: `tsAprobarCotizacion_Click()`
   - Agregado: `AbrirFormularioPresupuesto()` helper

6. **`SistemaPresupuestario\frmMain.Designer.cs`**
   - Vinculados eventos Click de menús

---

## ?? Validaciones Implementadas

### Validaciones Globales:
- ? ComboBox de estado siempre en solo lectura
- ? No se pueden editar presupuestos aprobados
- ? No se pueden eliminar presupuestos aprobados
- ? Los presupuestos emitidos no aparecen en Gestionar

### Validaciones en Modo GESTIONAR:
- ? Solo Borradores pueden emitirse
- ? Requiere al menos 1 artículo para emitir
- ? Solo Borradores pueden editarse/eliminarse
- ? Confirmación antes de emitir

### Validaciones en Modo APROBAR:
- ? Solo se muestran presupuestos Emitidos
- ? Confirmación con resumen antes de aprobar/rechazar
- ? No permite edición de ningún tipo

---

## ?? Notas de Diseño

### ¿Por qué 3 modos separados?

1. **Separación de Responsabilidades:**
   - Generar: Usuario común creando presupuestos
   - Gestionar: Administración general de presupuestos
   - Aprobar: Supervisor autorizando presupuestos

2. **Seguridad:**
   - Diferentes permisos por modo en el sistema de seguridad
   - Los usuarios solo ven lo que necesitan ver

3. **Usabilidad:**
   - Interfaz limpia según la tarea
   - No confundir con opciones no aplicables

### Extensiones Futuras

- [ ] Historial de cambios de estado
- [ ] Notificaciones cuando un presupuesto es aprobado/rechazado
- [ ] Vencimiento automático por fecha
- [ ] Dashboard de presupuestos por estado
- [ ] Exportación de presupuestos a PDF
- [ ] Firma digital en aprobación

---

## ?? Principios Aplicados

- **Single Responsibility:** Cada modo tiene una responsabilidad clara
- **Open/Closed:** Fácil agregar nuevos estados sin modificar código existente
- **Liskov Substitution:** Los modos son intercambiables en el formulario
- **DRY:** Código compartido en métodos helper
- **Clean Code:** Nombres descriptivos y documentación completa

---

**Autor:** Sistema de Presupuestos UAI  
**Fecha:** 2024  
**Versión:** 1.0
