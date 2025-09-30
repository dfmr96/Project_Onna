# Sistema de Movimiento - Documentación

## Descripción General

El sistema de movimiento del proyecto Onna consiste en tres scripts principales que trabajan en conjunto para proporcionar un control de movimiento fluido basado en input de teclado, aiming con mouse y visualización de debug:

- **PlayerMovement.cs**: Control principal de movimiento del jugador
- **MouseGroundAiming.cs**: Sistema de apuntado con mouse en el plano del suelo
- **InputVisualizerGizmos.cs**: Visualización y procesamiento de input con gizmos de debug

## Scripts del Sistema

### 1. PlayerMovement.cs
**Ubicación**: `Assets/Scripts/Movement/PlayerMovement.cs`

#### Propósito
Controla el movimiento del personaje del jugador con aceleración y desaceleración suaves, utilizando input procesado por el sistema de visualización.

#### Componentes Serializados
```csharp
[Header("Movement Settings")]
[SerializeField] private float moveSpeed = 5f;          // Velocidad base de movimiento
[SerializeField] private float acceleration = 10f;      // Velocidad de aceleración
[SerializeField] private float deceleration = 15f;      // Velocidad de frenado

[Header("References")]
[SerializeField] private InputVisualizerGizmos inputVisualizer;  // Referencia al visualizador
```

#### Variables Privadas
- `Vector3 currentMoveInput`: Input de movimiento actual aplicado
- `Vector3 smoothedMovement`: Movimiento suavizado para transiciones
- `Vector3 lastInputDirection`: Última dirección de input válida (para referencia)

#### Métodos Principales

##### `HandleMovement()`
- Obtiene el input relativo a la cámara desde `InputVisualizerGizmos`
- Determina la dirección de movimiento basada en input activo
- Aplica velocidad de movimiento y suavizado usando `Vector3.Lerp`
- Calcula tiempo de suavizado dinámico según aceleración/desaceleración

##### `MoveCharacter()`
- Aplica el movimiento final al `Transform` del personaje
- Usa `Time.deltaTime` para movimiento independiente del framerate
- Solo afecta el plano horizontal (XZ)

##### Métodos Públicos de Debug
- `GetCurrentVelocity()`: Retorna velocidad actual
- `GetSmoothedMovement()`: Retorna movimiento suavizado
- `GetLastInputDirection()`: Retorna última dirección válida

#### Gizmos Visuales
En `OnDrawGizmosSelected()` dibuja:
- **Azul**: Vector de movimiento actual
- **Cian**: Movimiento suavizado
- **Amarillo**: Última dirección de input

---

### 2. MouseGroundAiming.cs
**Ubicación**: `Assets/Scripts/MouseGroundAiming.cs`

#### Propósito
Maneja el sistema de apuntado con mouse proyectando rayos al plano del suelo, con soporte para modo cuerpo a cuerpo.

#### Componentes Serializados
```csharp
[Header("Targeting")]
public Transform aimTarget;              // Transform objetivo que sigue al mouse
public LayerMask groundLayerMask = 1;    // Máscara de capas para el suelo
public Camera playerCamera;              // Cámara del jugador

[Header("Ground Plane")]
public Transform playerTransform;        // Transform del jugador
public float planeOffset = 0f;          // Offset vertical del plano

[Header("Debug")]
public bool showDebugRay = true;        // Mostrar rayos de debug

[Header("Melee Mode")]
public bool isInMeleeMode = false;      // Estado del modo melee
```

#### Variables Privadas
- `Plane groundPlane`: Plano de proyección calculado dinámicamente
- `Vector3 savedMeleeTarget`: Posición guardada durante modo melee

#### Métodos Principales

##### `UpdateGroundPlane()`
- Actualiza el plano de proyección basado en la posición del jugador
- Aplica offset vertical configurable
- Se ejecuta cada frame para mantener el plano actualizado

##### `HandleMouseAiming()`
- **Modo Normal**: Proyecta rayo desde mouse al plano del suelo
- **Modo Melee**: Mantiene objetivo fijo en posición guardada
- Actualiza posición del `aimTarget` según el modo activo

##### `StartMeleeMode()` / `EndMeleeMode()`
- Control del estado de modo cuerpo a cuerpo
- Guarda/restaura posición objetivo
- Incluye logging de debug

#### Visualización Debug
- **Rayo Rojo**: Dirección del rayo desde cámara
- **Línea Verde**: Conexión exitosa al punto de impacto
- **Gizmos Azules**: Representación del plano de proyección
- **Esfera Amarilla**: Posición actual del objetivo

---

### 3. InputVisualizerGizmos.cs
**Ubicación**: `Assets/Scripts/Debug/InputVisualizerGizmos.cs`

#### Propósito
Procesa input de teclado, lo convierte a coordenadas relativas a la cámara y proporciona visualización completa con gizmos.

#### Componentes Serializados
```csharp
[Header("Gizmos Settings")]
[SerializeField] private bool showInputGizmos = true;   // Activar visualización
[SerializeField] private float gizmosScale = 2f;       // Escala de gizmos
[SerializeField] private Color inputColor = Color.green; // Color con input
[SerializeField] private Color noInputColor = Color.gray; // Color sin input

[Header("Input Keys")]
[SerializeField] private KeyCode forwardKey = KeyCode.W;   // Tecla adelante
[SerializeField] private KeyCode backwardKey = KeyCode.S;  // Tecla atrás
[SerializeField] private KeyCode leftKey = KeyCode.A;      // Tecla izquierda
[SerializeField] private KeyCode rightKey = KeyCode.D;     // Tecla derecha

[Header("Camera Reference")]
[SerializeField] private Camera cameraReference;         // Cámara para cálculos relativos

[Header("Aim Target Reference")]
[SerializeField] private Transform aimTarget;            // Objetivo de apuntado
[SerializeField] private bool showAngleGizmos = true;    // Mostrar ángulos
[SerializeField] private Color angleColor = Color.cyan;  // Color de ángulos

[Header("Move Target")]
[SerializeField] private Transform moveTarget;           // Objetivo de movimiento
[SerializeField] private bool updateMoveTarget = true;   // Actualizar automáticamente
```

#### Propiedades Públicas
- `Vector3 cameraRelativeInput { get; private set; }`: Input convertido a coordenadas de cámara
- `Vector2 GetCurrentInput()`: Input raw del teclado

#### Métodos Principales

##### `CalculateCameraRelativeInput()`
- Convierte input 2D del teclado a Vector3 en espacio 3D
- Proyecta vectores de cámara al plano horizontal
- Calcula dirección final basada en orientación de cámara

##### `UpdateMoveTarget()`
- Actualiza posición del `moveTarget` basado en input actual
- Posiciona el target en la punta del vector de input
- Se comporta dinámicamente según magnitud del input

#### Sistema de Visualización Avanzado

##### Gizmos de Input Principal
- **Flecha Verde**: Dirección y magnitud del input
- **Círculo**: Intensidad del input (radio proporcional)
- **Indicadores WASD**: Estado individual de cada tecla

##### Visualización de Ángulos
- **Línea Roja**: Dirección hacia objetivo de apuntado
- **Arco Cian**: Ángulo entre input y objetivo
- **Texto**: Valor del ángulo en grados

##### Move Target
- **Línea Magenta**: Conexión jugador-objetivo de movimiento
- **Esfera Blanca**: Posición del objetivo
- **Esfera Magenta**: Estado activo con input

## Flujo de Trabajo del Sistema

### 1. Procesamiento de Input
```
InputVisualizerGizmos.Update()
├── Captura teclas WASD
├── Normaliza input diagonal
├── CalculateCameraRelativeInput()
│   ├── Obtiene vectores de cámara
│   ├── Proyecta al plano horizontal
│   └── Calcula dirección final
└── UpdateMoveTarget()
```

### 2. Aplicación de Movimiento
```
PlayerMovement.Update()
├── HandleMovement()
│   ├── Obtiene cameraRelativeInput
│   ├── Calcula velocidad objetivo
│   ├── Aplica suavizado con Lerp
│   └── Actualiza currentMoveInput
└── MoveCharacter()
    └── Aplica movimiento al Transform
```

### 3. Sistema de Apuntado
```
MouseGroundAiming.Update()
├── UpdateGroundPlane()
├── HandleMouseAiming()
│   ├── Verifica modo melee
│   ├── Proyecta rayo mouse-plano
│   └── Actualiza aimTarget
└── Visualiza debug rays
```

## Configuración Recomendada

### PlayerMovement
- **moveSpeed**: 3-7 unidades/segundo para movimiento natural
- **acceleration**: 8-12 para respuesta rápida
- **deceleration**: 12-20 para paradas suaves

### MouseGroundAiming
- **planeOffset**: 0-0.5 para ajustar altura de apuntado
- **groundLayerMask**: Incluir solo capas de suelo navegable

### InputVisualizerGizmos
- **gizmosScale**: 1.5-3 para visualización clara
- **showInputGizmos**: true durante desarrollo, false en build

## Dependencias del Sistema

### Internas
- **PlayerMovement** → **InputVisualizerGizmos** (cameraRelativeInput)
- **InputVisualizerGizmos** → **MouseGroundAiming** (auto-detección de aimTarget)

### Unity
- `UnityEngine.Transform`
- `UnityEngine.Camera`
- `UnityEngine.Input`
- `UnityEngine.Gizmos`
- `UnityEditor.Handles` (solo editor)

## Notas de Desarrollo

### Rendimiento
- Todos los cálculos se ejecutan en `Update()` - considerar `FixedUpdate()` para física
- Los gizmos solo se renderizan en Scene view durante desarrollo
- Auto-detección de componentes reduce configuración manual

### Extensibilidad
- Sistema modular permite fácil adición de nuevos tipos de input
- Visualización de debug facilita tuning de parámetros
- Arquitectura preparada para animaciones y efectos visuales

### Debugging
- Múltiples capas de visualización para diferentes aspectos
- Logging automático en transiciones de modo melee
- Getters públicos para inspección en runtime