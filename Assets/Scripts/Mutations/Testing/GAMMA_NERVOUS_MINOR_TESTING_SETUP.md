# 🧪 Sistema de Testing para Gamma Nervous Minor

**Versión:** v1.0.0 ✅ **COMPLETO Y FUNCIONAL**
**Fecha:** 14 de Septiembre, 2025

---

## 📋 Resumen

Este sistema permite testear completamente la mutación **Gamma Nervous Minor** que mejora la atracción de orbes desde mayor distancia y velocidad. Incluye:

- ✅ **Testing de la mutación** con niveles 1-4
- ✅ **Spawning de orbes** para probar atracción
- ✅ **Visualización en tiempo real** del rango de atracción
- ✅ **Integración completa** con el sistema de stats del player
- ✅ **GUI visual** para monitoreo en tiempo real

![GammaNervousMinorEffect Graph](GammaNervousMinorEffect_Graph.jpg)
---

## 🛠️ Configuración del Testing System

### **Paso 1: Crear GameObject de Testing**

1. **En la escena de testing:**
   ```
   Hierarchy → Right-click → Create Empty
   Nombre: "GammaNervousMinorTester"
   ```

2. **Agregar el componente:**
   ```
   Add Component → GammaNervousMinorTestingSystem
   ```

### **Paso 2: Configurar Referencias**

En el Inspector del `GammaNervousMinorTestingSystem`:

| Campo | Valor | Descripción |
|-------|-------|-------------|
| **Gamma Nervous Minor Effect** | `GammaNervousMinor.asset` | Arrastra el ScriptableObject desde Project |
| **Mutation Level** | `1` | Nivel inicial de testing (1-4) |
| **Is Mutation Active** | `false` | Estado inicial |
| **Orb Prefab** | `RastroOrb` prefab | Prefab de orb para spawning |
| **Orbs To Spawn** | `5` | Cantidad de orbs por spawn |
| **Spawn Radius** | `15f` | Radio de spawn alrededor del player |
| **Spawn Height** | `1f` | Altura de spawn |
| **Show Attraction Range** | `true` | Mostrar gizmos de rango |
| **Attraction Range Color** | `Cyan` | Color de los gizmos |

### **Paso 3: Verificar Dependencias**

Asegúrate de que existen:
- ✅ `GammaNervousMinorEffect.cs` - La mutación
- ✅ `RastroOrb` prefab - Orbes para testing
- ✅ Player con `PlayerModel` component
- ✅ `StatReferences` con `orbAttractRange` y `orbAttractSpeed`

---

## 🎮 Controles de Testing

### **Hotkeys:**

| Tecla | Acción | Descripción |
|-------|--------|-------------|
| **K** | Toggle Mutation | Aplica/Remueve la mutación al level actual |
| **L** | Cycle Level | Cambia entre niveles 1-4 |
| **O** | Spawn Orbs | Crea orbs en círculo alrededor del player |
| **C** | Clear Orbs | Elimina todos los orbs de la escena |
| **G** | Toggle GUI | Muestra/Oculta la interfaz de debug |

### **GUI Visual:**

La GUI muestra:
- ✅ **Status**: ACTIVE/INACTIVE con nivel actual
- ✅ **Current Range**: Rango de atracción actual en metros
- ✅ **Current Speed**: Velocidad de atracción actual
- ✅ **Controles**: Lista de teclas disponibles

---

## 📊 Valores Esperados por Nivel

### **Tabla de Escalado:**

| Nivel | Rango Base | Rango Final | Velocidad | Descripción |
|-------|------------|-------------|-----------|-------------|
| **1** | 8.0m | +8.0m | x1.5 | `"Atrae orbes desde 8.0m a velocidad x1.5"` |
| **2** | 10.0m | +10.0m | x1.9 | `"Atrae orbes desde 10.0m a velocidad x1.9"` |
| **3** | 12.5m | +12.5m | x2.3 | `"Atrae orbes desde 12.5m a velocidad x2.3"` |
| **4** | 15.6m | +15.6m | x2.9 | `"Atrae orbes desde 15.6m a velocidad x2.9"` |

### **Fórmulas:**
```csharp
// Rango de atracción
attractRange = baseValue * Mathf.Pow(upgradeMultiplier, level - 1)
// baseValue = 8.0f, upgradeMultiplier = 1.25f

// Velocidad de atracción
attractSpeed = speedBase * Mathf.Pow(upgradeMultiplier, level - 1)
// speedBase = 1.5f, upgradeMultiplier = 1.25f
```

---

## 🔬 Procedimiento de Testing

### **Testing Básico:**

1. **Inicializar sistema:**
   ```
   1. Entrar en Play mode
   2. Verificar que GUI aparece (tecla G si no es visible)
   3. Confirmar que Player está en escena
   ```

2. **Probar aplicación de mutación:**
   ```
   1. Presionar K → Verificar "Status: ACTIVE (Level 1)"
   2. Verificar logs: "[GammaNervousMinorTesting] ✅ Mutation APPLIED"
   3. Observar cambios en Current Range y Current Speed
   ```

3. **Probar spawn de orbs:**
   ```
   1. Presionar O → Verificar que aparecen 5 orbs en círculo
   2. Observar que orbs se acercan al player cuando entra en rango
   3. Notar la velocidad de atracción aumentada
   ```

4. **Probar escalado de niveles:**
   ```
   1. Presionar L → Level cambia de 1 a 2
   2. Observar que stats se actualizan automáticamente
   3. Repetir hasta level 4, luego vuelve a 1
   ```

### **Testing Avanzado:**

1. **Verificar integración con RastroOrb:**
   ```
   1. Spawn orbs con mutación INACTIVE → Rango base ~5m
   2. Aplicar mutación Level 4 → Spawn nuevos orbs
   3. Verificar que nuevos orbs tienen mayor rango de atracción
   4. Comparar velocidad de acercamiento
   ```

2. **Testing de persistencia:**
   ```
   1. Aplicar mutación Level 3
   2. Spawn varios orbs
   3. Remover mutación (K)
   4. Verificar que stats vuelven a valores base
   5. Orbs existentes deben usar valores base inmediatamente
   ```

---

## 🎨 Visualización Debug

### **Gizmos en Scene View:**

- **Círculo Cyan**: Rango de atracción actual con mutación
- **Círculo Gris**: Rango base de referencia (5m)
- **Comparación visual** para validar que la mutación funciona

### **Console Logs:**

```
[GammaNervousMinorTesting] ✅ Mutation APPLIED - Level 2: Range 10.0m, Speed x1.9
[GammaNervousMinorTesting] 📊 After Apply Stats:
  Current Range: 10.0m | Expected: 10.0m
  Current Speed: 1.9x | Expected: 1.9x
[GammaNervousMinorTesting] 🌟 Spawned 5 orbs around player
[GammaNervousMinorTesting] 📊 Player Stats Applied to Orb - Range: +10.0m, Speed: +0.9x
```

---

## 🔧 Archivos Modificados

### **✅ Archivos Principales:**
- `GammaNervousMinorTestingSystem.cs` - Sistema de testing completo
- `RastroOrb.cs` - Integrado con player stats dinámicamente
- `GammaNervousMinorEffect.cs` - Mutación funcional
- `StatReferences.cs` - Stats orbAttractRange y orbAttractSpeed

### **✅ Integración RastroOrb:**
- **Propiedades dinámicas**: `attractionRadius` y `attractionSpeed` ahora leen player stats
- **Fallback seguro**: Si no hay player stats, usa valores base
- **Tiempo real**: Stats se actualizan cada frame automáticamente
- **Gizmos actualizados**: Visualización refleja valores reales

---

## 🧪 Casos de Testing Recomendados

### **Caso 1: Verificación de Escalado**
```
1. Level 1 → Spawn orbs → Medir rango visualmente
2. Level 4 → Spawn orbs → Comparar rango aumentado
3. Verificar que Level 4 atrae desde ~15.6m vs 8.0m de Level 1
```

### **Caso 2: Testing de Velocidad**
```
1. Sin mutación → Spawn orb cerca del límite base
2. Con mutación Level 4 → Spawn orb en misma posición
3. Comparar velocidad de acercamiento (debe ser ~2.9x más rápido)
```

### **Caso 3: Testing de Integración**
```
1. Aplicar mutación
2. Verificar PlayerStatsDebugger muestra valores correctos
3. Spawn orbs → Confirmar que usan esos valores
4. Remover mutación → Confirmar que orbs vuelven a base inmediatamente
```

---

## ⚠️ Troubleshooting

### **Problema: Orbs no se atraen**
**Solución:**
```
1. Verificar que RastroOrb prefab tiene Collider con IsTrigger = true
2. Confirmar que Player tiene PlayerModel component
3. Revisar que StatReferences tiene orbAttractRange/Speed asignados
```

### **Problema: GUI no aparece**
**Solución:**
```
1. Presionar G para toggle GUI
2. Verificar que GammaNervousMinorTestingSystem está en GameObject activo
3. Confirmar que está en Play mode
```

### **Problema: Stats no cambian**
**Solución:**
```
1. Verificar que PlayerModel.StatContext no es null
2. Confirmar que StatDefinitions existen en Assets/Stats/Definitions/
3. Revisar logs de Console para errores de aplicación
```

---

## 📞 Workflow de Testing

### **Para Desarrolladores:**
1. ✅ Configurar GameObject con `GammaNervousMinorTestingSystem`
2. ✅ Asignar referencias en Inspector
3. ✅ Usar hotkeys para testing rápido
4. ✅ Verificar logs y GUI para validación
5. ✅ Comparar con valores esperados en tabla

### **Para Game Designers:**
1. ✅ Ajustar valores base en `GammaNervousMinorEffect`
2. ✅ Modificar `upgradeMultiplier` para cambiar escalado
3. ✅ Testing visual con spawn de orbs
4. ✅ Balancear rango vs velocidad según gameplay
5. ✅ Validar que mutation se siente impactante pero balanceada

---

## 🎯 Estado Actual

- ✅ **Sistema de Testing**: Completamente funcional con hotkeys y GUI
- ✅ **Integración RastroOrb**: Orbs usan player stats dinámicamente
- ✅ **Visualización**: Gizmos y logs muestran valores en tiempo real
- ✅ **Escalado por niveles**: Funciona correctamente 1-4
- ✅ **Apply/Remove**: Sin bugs de acumulación
- ✅ **Performance**: Cálculos eficientes cada frame
- ✅ **Error handling**: Fallbacks seguros para casos edge

## 🚀 Funcionalidad Implementada

### **✅ Testing System Completo:**
- **Hotkeys intuitivos**: K, L, O, C, G para todas las funciones
- **GUI en tiempo real**: Stats actuales y controles visibles
- **Spawn circular**: 5 orbs equidistantes para testing consistente
- **Clear function**: Limpieza rápida de escena para re-testing

### **✅ RastroOrb Integration:**
- **Dynamic stats**: Lee player stats cada frame automáticamente
- **Seamless updates**: Cambios de stats se aplican inmediatamente
- **Safe fallbacks**: Funciona sin player stats (valores base)
- **Visual gizmos**: Rango actualizado visible en Scene View

### **✅ Mutation Validation:**
- **Level tracking**: Aplicación y remoción correcta por niveles
- **Real-time feedback**: Logs detallados de cada operación
- **Expected vs actual**: Comparación automática de valores
- **Edge case handling**: Manejo robusto de errores y null references

---

**¡Sistema de testing listo para usar! 🎮✨**