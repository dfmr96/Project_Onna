# **🧬 Categorización de Mutaciones - ONNA**

**Versión:** v1.0.0
**Fecha:** 14 de Septiembre, 2025

---

## **📊 Resumen de Categorías**

Basado en el análisis del documento de diseño, las **36 mutaciones** (6 radiaciones × 3 sistemas × 2 slots) se organizan en **7 categorías principales**:

| Categoría | Cantidad | Descripción | Ejemplo |
|-----------|----------|-------------|---------|
| **🔥 DOT Effects** | 12 | Daño continuo (quemaduras) | Microondas en todos los sistemas |
| **⚡ Aura Effects** | 8 | Campos de área alrededor del jugador | Gamma Tegumentario, Beta Tegumentario |
| **🎯 Projectile Modifiers** | 8 | Modifican comportamiento de disparos | Gamma/Alfa/Beta/Cherenkov Muscular |
| **📈 Stat Modifiers** | 4 | Buffs de estadísticas/movimiento | Beta Nervioso, Gamma Nervioso |
| **⏰ Time Vital Effects** | 12 | Gestión de tiempo vital | Neutrones en todos los sistemas |
| **🧲 Orb Interaction** | 4 | Interacción con orbes | Gamma/Cherenkov Nervioso |
| **📍 Marking Effects** | 12 | Marcado/debuff de enemigos | Cherenkov en todos los sistemas |

---

## **🔥 DOT Effects (Damage Over Time)**

### **Características Comunes:**
- Todas las mutaciones **Microondas**
- Duración: 1-3 segundos
- Algunas tienen **sinergia** (más daño si ya quemado)
- Aplicación: disparos, campos, ataques melee

### **Implementaciones:**
- **Microondas + Nervioso**: Disparos aplican quemadura
- **Microondas + Tegumentario**: Campo térmico alrededor del jugador
- **Microondas + Muscular**: Disparos con quemadura
- **Microondas + Melee**: Golpes aplican quemadura con sinergia
- **Microondas + Endocrino**: Explosiones dejan campos ardientes

### **Clase Base:** `DOTEffect`
```csharp
public abstract class DOTEffect : RadiationEffect
{
    protected float dotDuration;
    protected float tickRate;
    protected bool hasSynergy;
    protected float synergyMultiplier;
}
```

---

## **⚡ Aura Effects (Campos de Área)**

### **Características Comunes:**
- Radio de efecto alrededor del jugador
- Pueden ser **continuos** o por **intervalos**
- Afectan enemigos cercanos
- Escalado: radio y potencia aumentan con nivel

### **Implementaciones:**
- **Gamma Tegumentario**: Aura de daño radial continuo
- **Beta Tegumentario**: Campo de fricción (ralentización)
- **Microondas Tegumentario**: Campo térmico (quemadura)
- **Cherenkov Tegumentario**: Campo de debilitamiento
- **Alfa Tegumentario**: Ondas de contragolpe al recibir daño

### **Clase Base:** `AuraEffect`
```csharp
public abstract class AuraEffect : RadiationEffect
{
    protected float auraRadius;
    protected bool isContinuous;
    protected float intervalDuration;
    protected LayerMask enemyLayers;
}
```

---

## **🎯 Projectile Modifier Effects**

### **Características Comunes:**
- Modifican comportamiento de proyectiles
- Afectan daño, penetración, cadencia, efectos secundarios
- Se aplican a armas de fuego del jugador

### **Implementaciones:**
- **Gamma Muscular**: Penetración múltiple
- **Alfa Muscular**: Alto daño, baja cadencia
- **Beta Muscular**: Ralentización en impacto
- **Cherenkov Muscular**: Marcado de enemigos

### **Clase Base:** `ProjectileModifierEffect`
```csharp
public abstract class ProjectileModifierEffect : RadiationEffect
{
    protected ProjectileModifierType modifierType;
    protected float modifierMultiplier;
    protected int maxTargets;
    protected bool affectsSecondaryEffects;
}
```

---

## **📈 Stat Modifier Effects**

### **Características Comunes:**
- Modifican estadísticas del jugador
- Pueden ser **permanentes** o **temporales**
- Multiplican o suman valores base
- Afectan movimiento, curación, resistencias

### **Implementaciones:**
- **Beta Nervioso**: Velocidad de movimiento y dash
- **Gamma Nervioso**: Multiplicador de curación + drenaje de vida
- **Alfa Nervioso**: Invulnerabilidad temporal

### **Clase Base:** `StatModifierEffect`
```csharp
public abstract class StatModifierEffect : RadiationEffect
{
    protected StatModifierType statType;
    protected bool isMultiplier;
    protected bool isTemporary;
    protected float temporaryDuration;
}
```

---

## **⏰ Time Vital Effects**

### **Características Comunes:**
- Todas las mutaciones **Neutrones**
- Recuperan tiempo vital por acciones específicas
- Máximo: 120 segundos
- Algunas ralentizan consumo temporal

### **Implementaciones:**
- **Neutrones + Nervioso**: +2s/+5s por orbe recogido
- **Neutrones + Tegumentario**: 20% chance +2s al recibir daño
- **Neutrones + Muscular**: +3s por shots acertados, +10s por 10 kills
- **Neutrones + Melee**: +1s por kill melee
- **Neutrones + Endocrino**: +1s por enemigo alcanzado (máx +5s)

### **Clase Base:** `TimeVitalEffect`
```csharp
public abstract class TimeVitalEffect : RadiationEffect
{
    protected TimeVitalTrigger triggerType;
    protected float timeBonus;
    protected int maxTimeVital;
    protected bool canSlowConsumption;
}
```

---

## **🧲 Orb Interaction Effects**

### **Características Comunes:**
- Modifican interacción con orbes de curación
- Atracción, spawn extra, bonos de curación
- Solo en Sistema Nervioso

### **Implementaciones:**
- **Gamma Nervioso Menor**: Atracción aumentada
- **Cherenkov Nervioso**: Enemigos marcados sueltan orbes extra

### **Clase Base:** `OrbInteractionEffect`
```csharp
public abstract class OrbInteractionEffect : RadiationEffect
{
    protected OrbInteractionType interactionType;
    protected float attractionRange;
    protected int extraOrbsCount;
    protected float orbSpawnChance;
}
```

---

## **📍 Marking Effects**

### **Características Comunes:**
- Todas las mutaciones **Cherenkov**
- Marcan enemigos con efectos de debuff
- Aumentan/disminuyen daño recibido/infligido
- Duración temporal con posible stacking

### **Implementaciones:**
- **Cherenkov + Nervioso**: Orbes extra de enemigos marcados
- **Cherenkov + Tegumentario**: Campo que debilita enemigos
- **Cherenkov + Muscular**: Disparos marcan para más daño
- **Cherenkov + Melee**: Marcado con reducción de daño
- **Cherenkov + Endocrino**: Explosiones marcan área

### **Clase Base:** `MarkingEffect`
```csharp
public abstract class MarkingEffect : RadiationEffect
{
    protected MarkingTrigger triggerType;
    protected MarkingEffectType markEffect;
    protected float markDuration;
    protected float markIntensity;
    protected bool canStack;
}
```

---

## **🏭 Factory Pattern Mejorado**

### **CategorizedEffectFactory**
```csharp
public RadiationEffect CreateEffect(MutationType radiation, SystemType system, SlotType slot)
{
    // Busca por categoría primero, luego crea efecto específico
    string category = GetEffectCategory(radiation, system, slot);
    return CreateEffectByCategory(category, radiation, system, slot);
}
```

### **Ventajas del Sistema Categorizado:**
- **Reutilización**: Clases base compartidas entre efectos similares
- **Consistencia**: Comportamiento predecible por categoría
- **Escalabilidad**: Fácil agregar nuevos efectos a categorías existentes
- **Mantenimiento**: Cambios en clase base afectan toda la categoría
- **Debug**: Filtrado y análisis por tipo de efecto

---

## **🎯 Implementación Recomendada**

### **Paso 1: Crear Efectos Base por Categoría**
1. `DOTEffect` → `MicroondasNerviosoMajorEffect`
2. `AuraEffect` → `GammaTegumentarioMajorEffect`
3. `ProjectileModifierEffect` → `GammaMuscularMajorEffect`
4. etc.

### **Paso 2: Configurar Factory Categorizado**
1. Definir templates por categoría
2. Auto-categorización de efectos existentes
3. Validación de cobertura completa

### **Paso 3: Integrar con Sistema Existente**
1. Actualizar `MutationManager` para usar factory categorizado
2. Mantener compatibilidad con arquitectura actual
3. Agregar herramientas de debug por categoría

---

## **📊 Estadísticas de Cobertura**

| Sistema | DOT | Aura | Projectile | Stat | TimeVital | Orb | Marking | Total |
|---------|-----|------|------------|------|-----------|-----|---------|-------|
| **Nervioso** | 2 | 0 | 0 | 3 | 2 | 2 | 2 | 11 |
| **Tegumentario** | 2 | 4 | 0 | 0 | 2 | 0 | 2 | 10 |
| **Muscular** | 2 | 0 | 4 | 0 | 2 | 0 | 2 | 10 |
| **Melee** | 2 | 0 | 0 | 0 | 2 | 0 | 2 | 6 |
| **Endocrino** | 2 | 0 | 0 | 0 | 2 | 0 | 2 | 6 |
| **TOTAL** | **10** | **4** | **4** | **3** | **10** | **2** | **10** | **43** |

---

## **🔄 Próximos Pasos**

1. **Implementar efectos específicos** usando clases base categorizadas
2. **Crear factory categorizado** con auto-categorización
3. **Migrar efectos existentes** al nuevo sistema
4. **Agregar herramientas de debug** para análisis por categoría
5. **Documentar ejemplos** de cada categoría implementada