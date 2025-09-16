# Documentación del Sistema MVC - Player

## Arquitectura General

El sistema Player está implementado usando el patrón **Model-View-Controller (MVC)** para separar responsabilidades y mejorar la mantenibilidad del código.

## Componentes Principales

### 1. PlayerModel.cs
**Responsabilidad**: Estado centralizado y lógica de datos
- Mantiene el estado actual del jugador (posición, dirección, modo de juego)
- Notifica cambios a través de eventos C#
- Maneja lógica de daño, curación y muerte
- Gestiona estadísticas y contexto de stats

```csharp
// Eventos principales para notificar cambios
public event Action<GameMode> OnGameModeChanged;
public event Action<Vector3> OnMovementDirectionChanged;
public event Action<Vector3> OnRawMovementDirectionChanged;
public event Action<Vector3> OnAimDirectionChanged;
public event Action<bool> OnCanShootChanged;

// Estado centralizado
public GameMode CurrentGameMode { get; private set; }
public Vector3 MovementDirection { get; private set; }
public Vector3 AimDirection { get; private set; }
public bool CanShoot { get; private set; }
```

### 2. PlayerView.cs
**Responsabilidad**: Presentación y orquestación de sub-views
- Orquesta las sub-views especializadas (animación, arma, audio, efectos)
- Escucha eventos del PlayerModel y reacciona a cambios
- NO contiene lógica de negocio, solo presentación

```csharp
// Sub-views especializadas
private PlayerAnimationView animationView;
private PlayerWeaponView weaponView;
private PlayerAudioView audioView;
private PlayerEffectsView effectsView;

// Maneja eventos del modelo
private void HandleGameModeChanged(GameMode newMode) { ... }
private void HandleRawMovementDirectionChanged(Vector3 rawInput) { ... }
```

### 3. PlayerController.cs
**Responsabilidad**: Lógica de negocio e input processing
- Procesa input del jugador y lo convierte en comandos
- Maneja física de movimiento y colisiones
- Actualiza el PlayerModel con nuevos estados
- Gestiona acciones (dash, disparar, interactuar)

```csharp
// Procesa input y actualiza modelo
private void ProcessMovementLogic()
{
    _playerModel.SetMovementDirection(_inputDirection);
    _playerModel.SetRawMovementDirection(_rawInputDirection);
    _playerModel.SetPosition(transform.position);
}
```

### 4. PlayerAnimationView.cs
**Responsabilidad**: Animaciones especializadas
- Gestiona cambios de Animator Controllers según modo de juego
- Controla parámetros de animación (MoveX, MoveY, Speed)
- Separa lógica entre HUB (simple) y Combat (complejo)
- Lee input RAW directamente para evitar modificaciones por colisiones
- Maneja RigBuilder enable/disable según modo de juego

```csharp
// Métodos principales de animación
public void UpdateMovement(Vector3 moveDir, GameMode mode);           // Entrada general
public void UpdateHubMovement();                                      // HUB específico
public void UpdateCombatMovementWithAim(Vector3 moveDir, Vector3 aimDir); // Combat con aim
private void UpdateCombatMovement(Vector3 moveDir);                   // Combat básico (sin rotación)
```

## Flujo de Datos

```
Input → PlayerController → PlayerModel → PlayerView → Sub-Views
  ↓           ↓               ↓            ↓           ↓
Teclado/   Procesa        Almacena    Orquesta    Ejecuta
Mouse      Lógica         Estado      Vistas      Efectos
```

## Sistema de Input Dual

Para resolver problemas de colisiones que modificaban las animaciones:

1. **Input Convertido** (`MovementInput`): Para física y movimiento real
   - Pasa por `Utils.IsoVectorConvert()` para vista isométrica
   - Se modifica por sistema de colisiones (sliding en paredes)

2. **Input Raw** (`RawMovementInput`): Para animaciones
   - Input puro del teclado/gamepad
   - NO modificado por colisiones
   - Garantiza valores limpios (-1,0,1) para el animator

## Modos de Juego

### HUB Mode
- Animaciones simples sin arma
- Rotación hacia dirección de movimiento
- RigBuilder deshabilitado
- Solo animación "hacia adelante"

### Combat Mode
- Animaciones complejas con sistema de rigging
- Separación entre rotación del cuerpo y dirección de movimiento
- RigBuilder habilitado para control independiente del torso
- Escenarios: Forward, Backward, Strafe
- Cálculos basados en dot product entre input y aim direction

#### Comportamiento Específico en Combat:
- **Idle + Aim**: Solo torso se mueve, piernas permanecen estáticas
- **Movimiento**: Piernas siguen input del teclado (MoveX/MoveY), torso apunta independientemente
- **UpdateCombatMovement**: Solo parámetros de animación, NO rota visualRoot
- **UpdateCombatMovementWithAim**: Maneja separación completa torso/piernas

## Eventos y Comunicación

El sistema usa eventos C# para comunicación desacoplada:

```csharp
// PlayerModel notifica cambios
OnGameModeChanged?.Invoke(newMode);

// PlayerView escucha y reacciona
_playerModel.OnGameModeChanged += HandleGameModeChanged;

// PlayerAnimationView recibe instrucciones
animationView?.SetGameMode(newMode);
```

## Flujo de Animaciones Actualizado

### Inicialización en HUB Mode
```
PlayerModel.InitializeGameMode()
    → OnGameModeChanged?.Invoke(Hub)
    → PlayerView.HandleGameModeChanged(Hub)
    → animationView.SetGameMode(Hub)
    → UpdateAnimatorAndRigBuilder(Hub) // RigBuilder.Clear()
```

### Manejo de Movimiento en Combat
```
PlayerController → PlayerModel.SetRawMovementDirection()
    → OnRawMovementDirectionChanged?.Invoke(rawInput)
    → PlayerView.HandleRawMovementDirectionChanged(rawInput)
    → animationView.UpdateCombatMovementWithAim(rawInput, aimDirection)
    → RotateVisualRoot(legsDirection) + SetAnimationParameters()
```

## Troubleshooting

### Problema: Torso se mueve en idle + aim
- **Causa**: Se está usando `UpdateCombatMovement` en lugar de `UpdateCombatMovementWithAim`
- **Solución**: PlayerView debe llamar `UpdateCombatMovementWithAim` con aimDirection

### Problema: Personaje gira completo en movimiento
- **Causa**: `UpdateCombatMovement` está rotando el visualRoot
- **Solución**: `UpdateCombatMovement` solo debe establecer parámetros de animación

### Problema: RigBuilder no se inicializa correctamente
- **Causa**: `SetGameMode` no se está llamando en inicialización
- **Solución**: Verificar que `PlayerModel.InitializeGameMode()` se ejecuta al spawn
