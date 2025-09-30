# Documentación de MeleeController

## Descripción General

`MeleeController` es un sistema completo de combate cuerpo a cuerpo que se integra con el Animator Controller de Unity para proporcionar ataques combo precisos controlados por animaciones, con notificaciones de eventos en tiempo real y capacidades de depuración visual.

## Arquitectura Principal

### Diseño Basado en Eventos
El sistema utiliza eventos de C# para notificar a sistemas externos de cambios en el estado del combo:
```csharp
public event Action<int> OnComboStepChanged;  // Se dispara cuando cambia el paso del combo
public event Action OnComboReset;             // Se dispara cuando se reinicia el combo
```

### Integración con Animator
A diferencia de sistemas simples basados en temporizadores, MeleeController se sincroniza con el Animator Controller de Unity usando:
- **Parámetro ComboStep**: Un solo parámetro entero que controla todos los estados de animación
- **StateMachineBehaviours**: FirstAttackBehaviour y SecondAttackBehaviour manejan las transiciones de estado
- **Timing de Animación**: Las ventanas de combo se basan en duraciones reales de animación

## Gestión de Estados

### Estados del Combo
- **ComboStep = 0**: Estado inactivo/listo
- **ComboStep = 1**: Primer ataque ejecutándose, ventana de combo activa
- **ComboStep = 2**: Segundo ataque ejecutándose, combo completo

### Banderas de Estado
- **isAttacking**: Actualmente ejecutando una animación de ataque
- **onCoolDown**: En período de enfriamiento después de completar el combo
- **isInComboWindow**: La ventana de combo está activa y acepta entrada

## Uso

### Configuración Básica
1. **Asignar Referencias**: Arrastra MeleeData, Transform del punto de ataque, Animator y PlayerRigController
2. **Configurar Animator**: Crear estados para ComboStep 0, 1 y 2 con transiciones
3. **Agregar Behaviours**: Adjuntar FirstAttackBehaviour y SecondAttackBehaviour a sus respectivos estados

### Llamar Ataques
```csharp
meleeController.Attack(); // Llamar desde el sistema de entrada
```

### Suscribirse a Eventos
```csharp
meleeController.OnComboStepChanged += (step) => Debug.Log($"Paso de combo: {step}");
meleeController.OnComboReset += () => Debug.Log("Combo reiniciado");
```

## Diagrama de Flujo

```
[Inactivo] --Attack()--> [Primer Ataque] --VentanaCombo--> [Segundo Ataque] --Enfriamiento--> [Inactivo]
     ↑                         ↓                                 ↓
     ↑                    [Timeout]                         [Completo]
     ↗─────────────────────────┘                              ↓
                                                       [Enfriamiento]
```

## Métodos Clave

### Interfaz Pública
- **`Attack()`**: Método de entrada principal, maneja la lógica del combo y las transiciones de estado
- **`StartComboWindow(float duration)`**: Inicia la ventana de combo con timing basado en animación
- **`ExecuteDamage()`**: Realiza detección y aplicación de daño (llamado por StateMachineBehaviours)
- **`OnAnimationComplete()`**: Maneja eventos de finalización de animación desde behaviours

### Métodos Internos
- **`StopComboWindow()`**: Termina la ventana de combo activa
- **`ResetCombo()`**: Reinicia todo el estado del combo a inactivo y dispara evento OnComboReset
- **`DoCooldown()`**: Gestiona el período de enfriamiento post-combo

⚠️ **Nota Importante**: En la versión actual existe un bug en `OnAnimationComplete()` donde se llama `ResetCombo()` dos veces (líneas 164 y 168). Esto puede causar que el evento `OnComboReset` se dispare múltiples veces.

## Sistema de Ventana de Combo

### Mecanismo de Timing
1. **FirstAttackBehaviour** llama a `StartComboWindow(stateInfo.length)`
2. **ComboWindowCoroutine** se ejecuta durante la duración de la animación del primer ataque
3. Si el jugador presiona `Attack()` durante la ventana → ComboStep = 2
4. Si la ventana expira sin entrada → Reinicia al estado inactivo

### Duración de la Ventana
La duración de la ventana de combo se **calcula dinámicamente** desde la longitud de la animación del primer ataque:
```csharp
meleeController.StartComboWindow(stateInfo.length); // Desde FirstAttackBehaviour
```

## Sistema de Depuración

### Interfaz de Depuración Visual
Habilita `showComboDebug` en el Inspector para mostrar:
- **Paso del Combo**: Estado actual del combo (0, 1, 2)
- **Banderas de Estado**: Estado de isAttacking, onCoolDown
- **Barra de Progreso**: Cuenta regresiva de la ventana de combo en tiempo real con codificación de colores
- **Retroalimentación de Entrada**: Flash visual cuando se detecta entrada de combo

### Controles de Depuración
```csharp
[Header("Debug")]
[SerializeField] private bool showComboDebug = false;  // Alternar UI de debug
private bool showGizmo = false;                        // Alternar gizmo de rango de ataque
```

## Puntos de Integración

### Componentes Requeridos
- **Animator**: Debe tener el parámetro entero ComboStep
- **PlayerRigController**: Para gestión de estados de rig durante ataques
- **MeleeData**: ScriptableObject que contiene parámetros de ataque
- **StateMachineBehaviours**: FirstAttackBehaviour y SecondAttackBehaviour

### Dependencias
- **MeleeModel**: Maneja datos y cálculos de ataque
- **IDamageable**: Interfaz para objetos que pueden recibir daño
- **Transform attackPoint**: Posición mundial para detección de esfera de daño

## Configuración

### Configuración del Inspector
```
[Header("Data")]
- MeleeData: ScriptableObject con parámetros de ataque
- Attack Point: Transform que marca el centro de detección de daño

[Header("References")]
- Animator: Animator Controller del personaje
- Player Rig Controller: Para gestión de rig de animación

[Header("Debug")]
- Show Combo Debug: Alternar interfaz de depuración visual
```

### Requisitos del Animator Controller
1. **Parámetro ComboStep**: Parámetro entero que controla transiciones de estado
2. **Estados**: Inactivo (ComboStep=0), PrimerAtaque (ComboStep=1), SegundoAtaque (ComboStep=2)
3. **Transiciones**: Basadas en cambios de valor de ComboStep
4. **Behaviours**: FirstAttackBehaviour en estado de primer ataque, SecondAttackBehaviour en estado de segundo ataque

## Consideraciones de Rendimiento

### Gestión Eficiente de Estados
- Los eventos solo se disparan cuando los valores realmente cambian (patrón property wrapper)
- Las corrutinas se limpian apropiadamente para prevenir fugas de memoria
- Las referencias de componentes se cachean y auto-resuelven

### Detección de Daño
- Usa `Physics.OverlapSphere()` para detección eficiente de colisiones
- Filtra capas específicas (capa 6) para evitar auto-daño
- Solo se ejecuta durante frames reales de ataque vía StateMachineBehaviours

## Problemas Comunes y Soluciones

### Combo No Se Activa
- **Verificar Animator**: Asegurar que el parámetro ComboStep existe y las transiciones están configuradas
- **Verificar Behaviours**: FirstAttackBehaviour debe estar adjunto al estado de primer ataque
- **Ventana de Debug**: Habilitar showComboDebug para verificar timing de ventana de combo

### Problemas de Timing de Animación
- **Duración de Estado**: Asegurar que los clips de animación tienen la longitud correcta
- **Timing de Behaviour**: FirstAttackBehaviour.OnStateEnter inicia la ventana de combo
- **Duración de Ventana**: Basada en `stateInfo.length`, verificar timing de animación

### Eventos No Se Disparan
- **Suscripción**: Asegurar que los eventos están suscritos antes de llamar Attack()
- **Cambios de Valor**: Los eventos solo se disparan cuando ComboStep realmente cambia de valor
- **Ciclo de Vida**: Los eventos se disparan durante el setter de propiedad, no las llamadas de método

## Ejemplo de Uso

```csharp
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private MeleeController meleeController;

    private void Start()
    {
        // Suscribirse a eventos de combo
        meleeController.OnComboStepChanged += OnComboChanged;
        meleeController.OnComboReset += OnComboReset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            meleeController.Attack();
        }
    }

    private void OnComboChanged(int step)
    {
        Debug.Log($"Paso de combo cambió a: {step}");
        // Actualizar UI, reproducir efectos, etc.
    }

    private void OnComboReset()
    {
        Debug.Log("Combo reiniciado a inactivo");
        // Reiniciar UI, detener efectos, etc.
    }
}
```

## Historial de Versiones

### Versión Actual (v2.0)
- Reescritura completa con integración de Animator
- Arquitectura basada en eventos
- Sistema de depuración visual
- Timing basado en animación

### Versión Anterior (v1.0)
- Sistema simple basado en corrutinas
- Detección de combo basada en temporizador
- Aplicación básica de daño
- Sin sistema de eventos externos

## Problemas Conocidos

### Bug de Doble ResetCombo (v2.0)
**Ubicación**: `OnAnimationComplete()` líneas 164 y 168
**Descripción**: Se llama `ResetCombo()` dos veces, causando:
- Disparo múltiple del evento `OnComboReset`
- Potencial confusión en sistemas que escuchan este evento
- Lógica redundante

**Solución recomendada**:
```csharp
public void OnAnimationComplete()
{
    Debug.Log($"[MeleeController] Animación completa step {ComboStep}");
    showGizmo = false;

    if (ComboStep == 2)
    {
        ResetCombo();
        StartCoroutine(DoCooldown());
    }
    else
    {
        ResetCombo(); // Solo llamar si no es el segundo ataque
    }
}
```

### Comportamiento del comboStepParam
**Descripción**: El parámetro `comboStepParam` se define pero no se usa en el código actual
**Impacto**: El Animator Controller debe actualizarse manualmente
**Estado**: Funcionalidad implementada pero no utilizada activamente

### Problemas de Rendimiento Potenciales
**GetComponent en StateMachineBehaviours**: Los behaviours llaman `GetComponent<>()` en cada entrada/salida de estado
**Solución recomendada**: Cachear referencias de componentes en los behaviours

### Gestión de Corrutinas
**Descripción**: Múltiples corrutinas pueden ejecutarse simultáneamente si se llama Attack() rápidamente
**Impacto**: Potencial comportamiento inesperado en combos rápidos
**Estado**: Mitigado parcialmente por verificaciones de estado, pero podría mejorarse