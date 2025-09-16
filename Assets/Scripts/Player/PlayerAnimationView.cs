using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    /// <summary>
    /// PlayerAnimationView - Sub-view especializada en manejar animaciones del jugador
    /// </summary>
    public class PlayerAnimationView : MonoBehaviour
    {
        [Header("Animation References")] 
        [SerializeField] private Animator animator;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private RigBuilder rigBuilder;

        [Header("Input Reference")] 
        [SerializeField] private PlayerInputHandler inputHandler;
        

        [Header("Animation Controllers")] 
        [SerializeField] private RuntimeAnimatorController hubAnimator;
        [SerializeField] private RuntimeAnimatorController combatAnimator;

        [Header("Animation Parameters")] 
        [SerializeField] private float rotationSpeed = 501f;
        
        // Cached animation parameter hashes
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        private GameMode _currentMode;

        /// <summary>
        /// Actualiza las animaciones según el modo de juego
        /// Punto de entrada principal para actualizaciones de movimiento
        /// </summary>
        public void SetGameMode(GameMode mode)
        {
            if (_currentMode == mode) return;

            _currentMode = mode;

            UpdateAnimatorAndRigBuilder(mode);
            Debug.Log("Game Mode: " + _currentMode);
        }

        private void UpdateAnimatorAndRigBuilder(GameMode mode)
        {
            Debug.Log("Builder y animator actualizados para modo: " + mode);
            // Cambiar animator controller
            if (mode == GameMode.Hub && hubAnimator != null)
            {
                animator.runtimeAnimatorController = hubAnimator;
            }
            else if (mode != GameMode.Hub && combatAnimator != null)
            {
                animator.runtimeAnimatorController = combatAnimator;
            }

            // Configurar Rig Builder
            if (rigBuilder != null)
            {
                bool rigEnabled = mode != GameMode.Hub;
                rigBuilder.enabled = rigEnabled;

                if (rigEnabled)
                {
                    rigBuilder.Build();
                }
                else
                {
                    rigBuilder.Clear();
                }
            }
        }
        /// <summary>
        /// Actualiza las animaciones de movimiento según el modo de juego.
        /// Este es el punto de entrada principal para el movimiento.
        /// </summary>
        /// <param name="moveDir">Dirección de movimiento (para rotación visual en HUB o cálculo de animación en Combat)</param>
        /// <param name="aimDir">Dirección de apuntado (solo usado en Combat)</param>
        /// <param name="mode">Modo de juego actual (Hub o Combat)</param>
/// <summary>
        /// Actualiza las animaciones según el modo de juego
        /// Punto de entrada principal para actualizaciones de movimiento
        /// </summary>
        /// <param name="moveDir">Dirección de movimiento (puede estar modificada por colisiones)</param>
        /// <param name="mode">Modo de juego actual</param>
        public void UpdateMovement(Vector3 moveDir, GameMode mode)
        {
            if (mode == GameMode.Hub)
            {
                UpdateHubMovement();
            }
            else
            {
                // En Combat mode, siempre usar el método con aim para separar torso y piernas
                // El aimDirection se debe pasar desde quien llama este método
                Debug.LogWarning("UpdateMovement en Combat mode - usar UpdateCombatMovementWithAim directamente");
                UpdateCombatMovement(moveDir);
            }
        }
        

        /// <summary>
        /// Actualiza las animaciones y orientación del jugador para movimiento en HUB.
        /// Convierte el input raw al espacio de la cámara para ajustar la rotación y aplica parámetros de animación simplificados.
        /// </summary>
        public void UpdateHubMovement()
        {
            // Leer RAW INPUT directamente del InputHandler
            Vector3 rawInput = inputHandler.RawMovementInput;

            float speed = rawInput.magnitude;

            if (speed < 0.01f)
            {
                // Idle en HUB
                SetAnimationParameters(0f, 0f, 0f);
                return;
            }

            // Convertir raw input al espacio de la cámara para rotación correcta
            Vector3 isoRawInput = Utils.IsoVectorConvert(rawInput);
            RotateVisualRoot(isoRawInput);
            
            // Animación simple: solo hacia adelante cuando se mueve
            SetAnimationParameters(speed, 0f, 1f);
        }

/// <summary>
        /// Animaciones básicas para modo Combat (sin aim)
        /// SOLO establece parámetros de animación, NO rota el visualRoot
        /// En combat mode, el RigBuilder y el aim controlan la rotación del torso
        /// </summary>
        /// <param name="moveDir">Dirección de movimiento (no utilizada, se usa raw input)</param>
        private void UpdateCombatMovement(Vector3 moveDir)
        {
            // Leer RAW INPUT directamente del InputHandler
            Vector3 rawInput = inputHandler.RawMovementInput;

            if (rawInput.sqrMagnitude < 0.01f)
            {
                SetAnimationParameters(0f, 0f, 0f);
                return;
            }

            // IMPORTANTE: En combat mode NO rotar visualRoot
            // Solo establecer parámetros de animación para que las piernas respondan al input
            // La rotación del torso la maneja el RigBuilder con el aim
            SetAnimationParameters(rawInput.magnitude, rawInput.x, rawInput.z);
        }


        /// <summary>
        /// Actualiza el movimiento de combate del jugador considerando la dirección de apuntado.
        /// Este método es crucial para escenarios donde el jugador se mueve en una dirección
        /// considerando si se está apuntando hacia adelante, hacia atrás o haciendo strafe.
        /// </summary>
        /// <param name="moveDir"> El vector de dirección de movimiento deseado en el espacio mundial.</param>
        /// <param name="aimDir"> La dirección de apuntado del jugador en el espacio mundial.</param>
        public void UpdateCombatMovementWithAim(Vector3 moveDir, Vector3 aimDir)
        {
            // Leer RAW INPUT directamente del InputHandler
            Vector3 rawInput = inputHandler.RawMovementInput;

            if (rawInput.sqrMagnitude < 0.01f)
            {
                SetAnimationParameters(0f, 0f, 0f);
                return;
            }

            rawInput.y = 0;
            aimDir.y = 0;

            // Convertir solo el RAW INPUT al espacio de la cámara
            // El aimDir ya viene en coordenadas mundo correctas desde PlayerController
            Vector3 isoRawInput = Utils.IsoVectorConvert(rawInput);
            Vector3 worldAimDir = aimDir;

            // Calcular producto punto para determinar relación entre movimiento y apuntado
            float dotProduct = Vector3.Dot(isoRawInput.normalized, worldAimDir.normalized);

            Vector3 legsDirection;
            Vector3 relativeMovement;

            // Escenario 1: Movimiento hacia adelante (dot > 0.7)
            if (dotProduct > 0.7f)
            {
                legsDirection = worldAimDir;
                relativeMovement = Vector3.forward;
            }
            // Escenario 3: Movimiento hacia atrás (dot < -0.7)
            else if (dotProduct < -0.7f)
            {
                legsDirection = worldAimDir;
                relativeMovement = Vector3.back;
            }
            // Escenario 2: Movimiento lateral/strafe
            else
            {
                legsDirection = worldAimDir;
                // Calcular movimiento relativo al aim direction para strafe
                Vector3 aimRight = Vector3.Cross(worldAimDir, Vector3.up);
                float rightAmount = Vector3.Dot(isoRawInput, aimRight);
                float forwardAmount = Vector3.Dot(isoRawInput, worldAimDir);
                relativeMovement = new Vector3(rightAmount, 0f, forwardAmount);
            }

            // Rotar las piernas
            RotateVisualRoot(legsDirection);
            // Actualizar parámetros de animación
            SetAnimationParameters(isoRawInput.magnitude, relativeMovement.x, relativeMovement.z);
        }

        /// <summary>
        /// Rota el visual root del jugador hacia la dirección especificada de manera suave.
        /// </summary>
        /// <param name="direction"> La dirección objetivo para que el visual root mire. Típicamente derivada del movimiento o input de apuntado.</param>
        private void RotateVisualRoot(Vector3 direction)
        {
            if (visualRoot == null || direction.sqrMagnitude < 0.01f)
                return;

            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            visualRoot.rotation = Quaternion.Slerp(
                visualRoot.rotation,
                targetRotation, 
                Time.deltaTime * rotationSpeed
            );
        }

        /// <summary>
        /// Establece parámetros del Animator usando hashes cacheados
        /// </summary>
        /// <param name="speed">Magnitud del movimiento.</param>
        /// <param name="moveX">Movimiento lateral (-1 izquierda, 1 derecha).</param>
        /// <param name="moveY">Movimiento adelante/atrás (-1 atrás, 1 adelante).</param>
        private void SetAnimationParameters(float speed, float moveX, float moveY)
        {
            animator.SetFloat(Speed, speed);
            animator.SetFloat(MoveX, moveX); 
            animator.SetFloat(MoveY, moveY);
        }

        public void Initialize()
        {
            UpdateAnimatorAndRigBuilder(_currentMode);
        }
    }
}