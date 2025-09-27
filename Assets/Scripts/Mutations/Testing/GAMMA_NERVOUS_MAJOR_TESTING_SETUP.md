# 🧪 Sistema de Testing para Gamma Nervous Major

**Versión:** v1.0.0 ✅ **COMPLETO Y FUNCIONAL**
**Fecha:** 14 de Septiembre, 2025

---

## 📋 Resumen

Este sistema permite testear completamente la mutación **Gamma Nervous Major** que aumenta la curación de orbes pero incrementa el drenaje de vida. Incluye:

- ✅ **Testing de la mutación** con niveles 1-4
- ✅ **Testing de healing** con diferentes cantidades
- ✅ **Monitoreo en tiempo real** del multiplicador y drenaje
- ✅ **Integración completa** con el sistema de stats del player
- ✅ **GUI visual** para control y monitoreo

---

## 🛠️ Configuración del Testing System

### **Paso 1: Crear GameObject de Testing**

1. **En la escena de testing:**
   ```
   Hierarchy → Right-click → Create Empty
   Nombre: "GammaNervousMajorTester"
   ```

2. **Agregar el componente:**
   ```
   Add Component → GammaNervousTester
   ```

### **Paso 2: Configurar Referencias**

En el Inspector del `GammaNervousTester`:

| Campo | Valor | Descripción |
|-------|-------|-------------|
| **Gamma Nervous Effect** | `GammaNervousMajor.asset` | Arrastra el ScriptableObject desde Project |
| **Mutation Level** | `1` | Nivel inicial de testing (1-4) |
| **Is Mutation Active** | `false` | Estado inicial |
| **GUI Enabled** | `true` | Mostrar interfaz de debug |

### **Paso 3: Verificar Dependencias**

Asegúrate de que existen:
- ✅ `GammaNervousMajorEffect.cs` - La mutación
- ✅ Player con `PlayerModel` component
- ✅ `StatReferences` con `healingMultiplier` y `passiveDrainRate`
- ✅ `HealingMultiplier.asset` - StatDefinition

---

## 🎮 Controles de Testing

### **Hotkeys:**

| Tecla | Acción | Descripción |
|-------|--------|-------------|
| **M** | Toggle Mutation | Aplica/Remueve la mutación al level actual |
| **H** | Give Health (Large) | Otorga 10.0 puntos de vida para testing |
| **N** | Give Health (Small) | Otorga 2.0 puntos de vida para testing |
| **B** | Toggle GUI | Muestra/Oculta la interfaz de debug |

### **GUI Visual:**

La GUI muestra:
- ✅ **Status**: ACTIVE/INACTIVE con nivel actual
- ✅ **Healing Multiplier**: Multiplicador actual de curación
- ✅ **Drain Rate**: Incremento de drenaje actual
- ✅ **Expected vs Current**: Valores esperados vs reales
- ✅ **Controles**: Lista de teclas disponibles

---

## 📊 Valores Esperados por Nivel

### **Tabla de Escalado:**

| Nivel | Healing Multiplier | Drain Increase | Descripción |
|-------|-------------------|----------------|-------------|
| **1** | x1.50 | +0.5/s | `"Aumenta curación x1.5 pero incrementa drenaje +0.5/s"` |
| **2** | x1.95 | +1.0/s | `"Aumenta curación x1.9 pero incrementa drenaje +1.0/s"` |
| **3** | x2.54 | +1.5/s | `"Aumenta curación x2.5 pero incrementa drenaje +1.5/s"` |
| **4** | x3.30 | +2.0/s | `"Aumenta curación x3.3 pero incrementa drenaje +2.0/s"` |

### **Fórmulas:**
```csharp
// Multiplicador de curación
healingMultiplier = baseValue * Mathf.Pow(upgradeMultiplier, level - 1)
// baseValue = 1.5f, upgradeMultiplier = 1.3f

// Incremento de drenaje
drainIncrease = healthDrainIncrease * level
// healthDrainIncrease = 0.5f
```

---

## 🔬 Procedimiento de Testing

### **Testing Básico:**

1. **Inicializar sistema:**
   ```
   1. Entrar en Play mode
   2. Verificar que GUI aparece (tecla B si no es visible)
   3. Confirmar que Player está en escena
   4. Verificar valores base en PlayerStatsDebugger
   ```

2. **Probar aplicación de mutación:**
   ```
   1. Presionar M → Verificar "Status: ACTIVE (Level 1)"
   2. Verificar logs: "[GammaNervousTester] ✅ Mutation applied"
   3. Observar cambios en Healing Multiplier y Drain Rate
   4. Confirmar valores en PlayerStatsDebugger
   ```

3. **Probar healing aumentado:**
   ```
   1. Con mutación Level 3 activa (x2.54 multiplier)
   2. Presionar H → Dar 10 puntos de vida
   3. Observar que se recuperan ~25.4 puntos
   4. Verificar logs de healing con multiplier aplicado
   ```

4. **Probar escalado de niveles:**
   ```
   1. Cambiar mutationLevel en Inspector (1-4)
   2. Presionar M para re-aplicar
   3. Observar que stats se actualizan según tabla
   4. Verificar que drenaje aumenta linealmente
   ```

### **Testing Avanzado:**

1. **Verificar healing real:**
   ```
   1. Sin mutación → H → Verificar healing normal
   2. Level 1 → H → Verificar 1.5x healing
   3. Level 4 → H → Verificar 3.3x healing
   4. Comparar logs para confirmar multiplicador
   ```

2. **Testing de drenaje:**
   ```
   1. Aplicar mutación Level 2 (+1.0/s drain)
   2. Observar en PlayerStatsDebugger el valor actualizado
   3. Dejar pasar tiempo → Verificar drenaje aumentado
   4. Remover mutación → Confirmar vuelta a base
   ```

3. **Testing de persistencia:**
   ```
   1. Aplicar mutación Level 4
   2. Dar vida múltiples veces (H, N)
   3. Verificar que multiplier se mantiene
   4. Remover mutación → Confirmar healing vuelve a normal
   ```

---

## 🎨 Visualización Debug

### **GUI en Game View:**

```
┌─────────────────────────────────────┐
│ Gamma Nervous Major Tester          │
├─────────────────────────────────────┤
│ Status: ACTIVE (Level 3)            │
│ Healing Multiplier: 2.54x          │
│ Drain Rate: +1.5/s                 │
│                                     │
│ Expected Healing: 2.54x             │
│ Expected Drain: +1.5/s              │
│                                     │
│ Controls:                           │
│ M - Toggle Mutation                 │
│ H - Give Health (10.0)              │
│ N - Give Health (2.0)               │
│ B - Toggle This GUI                 │
└─────────────────────────────────────┘
```

### **Console Logs:**

```
[GammaNervousTester] ✅ Mutation applied at Level 3
[GammaNervousTester] Applied: Healing x2.54, Drain +1.5/s
[GammaNervousTester] 📊 Stats Update:
  Expected Healing: 2.54x | Current: 2.54x
  Expected Drain: 1.5/s | Current: 1.5/s
[GammaNervousTester] 💚 Gave 10.0 health → Recovered ~25.4 (with x2.54 multiplier)
[PlayerModel] RecoverTime: 10.0 base → 25.4 actual (HealingMultiplier: 2.54)
```

### **PlayerStatsDebugger Integration:**

- **Healing Multiplier**: Visible en tiempo real
- **Passive Drain Rate**: Actualizado automáticamente
- **Valores B/M/R**: Base, Meta, Runtime mostrados
- **Color coding**: Verde para positivos, rojo para negativos

---

## 🔧 Casos de Testing Recomendados

### **Caso 1: Verificación de Escalado de Healing**
```
1. Level 1 → H → Verificar ~15.0 healing (10 * 1.5)
2. Level 2 → H → Verificar ~19.5 healing (10 * 1.95)
3. Level 3 → H → Verificar ~25.4 healing (10 * 2.54)
4. Level 4 → H → Verificar ~33.0 healing (10 * 3.30)
```

### **Caso 2: Verificación de Drenaje**
```
1. Sin mutación → Observar drenaje base en debugger
2. Level 1 → Verificar +0.5/s incremento
3. Level 4 → Verificar +2.0/s incremento
4. Remover → Confirmar vuelta a drenaje base
```

### **Caso 3: Testing de Balance**
```
1. Level 4 → Máximo healing (x3.3) vs máximo drain (+2.0/s)
2. Dar vida repetidamente → ¿Compensa el healing extra?
3. Observar balance entre beneficio vs penalización
4. Ajustar valores si es necesario para balance
```

### **Caso 4: Testing de Integración**
```
1. Aplicar mutación
2. Usar orbes reales del juego (si disponibles)
3. Verificar que RecoverTime() aplica multiplier
4. Confirmar que logs muestran valores correctos
```

---

## ⚠️ Troubleshooting

### **Problema: Healing multiplier no funciona**
**Solución:**
```
1. Verificar que PlayerModel.HealingMultiplier property existe
2. Confirmar que RecoverTime() usa el multiplier
3. Revisar que StatContext no es null
4. Verificar HealingMultiplier.asset en StatDefinitions
```

### **Problema: Drenaje no aumenta**
**Solución:**
```
1. Verificar que passiveDrainRate stat existe en StatReferences
2. Confirmar que PlayerModel usa este stat para drenaje
3. Revisar aplicación en ApplyStatModification()
4. Usar PlayerStatsDebugger para verificar valores
```

### **Problema: GUI no aparece**
**Solución:**
```
1. Presionar B para toggle GUI
2. Verificar que guiEnabled = true en Inspector
3. Confirmar que está en Play mode
4. Revisar que GameObject tiene GammaNervousTester component
```

### **Problema: Logs no aparrecen**
**Solución:**
```
1. Abrir Console window (Window → General → Console)
2. Verificar que no hay filtros activos
3. Confirmar que Debug.Log statements están en código
4. Revisar que no hay errores bloqueando ejecución
```

---

## 📁 Archivos Involucrados

### **✅ Archivos Principales:**
- `GammaNervousTester.cs` - Sistema de testing principal
- `GammaNervousMajorEffect.cs` - La mutación a testear
- `PlayerModel.cs` - Integrado con healing multiplier
- `PlayerStatsDebugger.cs` - Visualización de stats

### **✅ Archivos de Configuración:**
- `StatReferences.cs` - Referencias a healing y drain stats
- `HealingMultiplier.asset` - StatDefinition para healing
- `PlayerBaseStats.asset` - Stats base del player

### **✅ Testing Dependencies:**
- Player GameObject con PlayerModel component
- StatContext inicializado correctamente
- PlayerStatsDebugger para visualización

---

## 🔄 Workflow de Testing

### **Para Desarrolladores:**
1. ✅ Configurar GameObject con `GammaNervousTester`
2. ✅ Asignar GammaNervousMajorEffect en Inspector
3. ✅ Usar hotkeys para testing rápido (M, H, N, B)
4. ✅ Verificar logs y PlayerStatsDebugger
5. ✅ Comparar valores esperados vs actuales

### **Para Game Designers:**
1. ✅ Ajustar `healingMultiplierBase` para balance inicial
2. ✅ Modificar `healthDrainIncrease` para penalización
3. ✅ Cambiar `upgradeMultiplier` para escalado
4. ✅ Testing de balance con diferentes niveles
5. ✅ Validar que trade-off healing vs drain es balanceado

---

## 🎯 Mejoras Futuras Sugeridas

### **Sistema de Testing Extendido:**
1. **Auto-testing**: Scripts que prueban todos los niveles automáticamente
2. **Performance testing**: Verificar que no hay memory leaks
3. **Integration testing**: Testing con otros sistemas del juego
4. **Balance validation**: Verificar que valores están dentro de rangos esperados

### **UI Improvements:**
1. **Sliders**: Para cambiar nivel en tiempo real
2. **Buttons**: Para aplicar/remover sin hotkeys
3. **Charts**: Visualización gráfica del escalado
4. **Presets**: Configuraciones predefinidas para testing

---

## 🎯 Estado Actual

- ✅ **Sistema de Testing**: Completamente funcional con hotkeys y GUI
- ✅ **Healing Multiplier**: Funciona correctamente en RecoverTime()
- ✅ **Drain Rate**: Se aplica correctamente al stat del player
- ✅ **Level Scaling**: Escalado correcto para niveles 1-4
- ✅ **Apply/Remove**: Sin bugs de acumulación
- ✅ **GUI Integration**: PlayerStatsDebugger muestra valores actualizados
- ✅ **Logging**: Información detallada en Console
- ✅ **Error Handling**: Manejo robusto de casos edge

## 🚀 Funcionalidad Validada

### **✅ Core Functionality:**
- **Healing multiplicador**: 1.5x → 1.95x → 2.54x → 3.30x por niveles
- **Drain incremento**: +0.5/s → +1.0/s → +1.5/s → +2.0/s por niveles
- **Real-time updates**: Cambios instantáneos en PlayerStatsDebugger
- **Persistent effects**: Efectos se mantienen hasta remoción manual

### **✅ Testing Validation:**
- **Hotkey controls**: M, H, N, B funcionan correctamente
- **GUI feedback**: Status y valores visibles en tiempo real
- **Console logging**: Información detallada de cada operación
- **PlayerModel integration**: RecoverTime() aplica multiplier correctamente

### **✅ Quality Assurance:**
- **Level tracking**: Aplicación y remoción correcta por niveles
- **No accumulation**: Sin bugs de valores acumulativos
- **Safe fallbacks**: Manejo de casos donde StatContext es null
- **Performance**: Cálculos eficientes sin impacto en framerate

---

**¡Sistema de testing completo y funcional para Gamma Nervous Major! 🧪⚡**