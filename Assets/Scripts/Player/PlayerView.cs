using UnityEngine;

namespace Player
{
    /// <summary>
    /// Componente View del patrón MVC que maneja la presentación
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        [Header("Sub-Views")] 
        [SerializeField] private PlayerAnimationView animationView;
        [SerializeField] private PlayerWeaponView weaponView;
        [SerializeField] private MeleeView meleeView;
        [SerializeField] private PlayerAudioView audioView;
        

        [SerializeField] private PlayerEffectsView effectsView;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerModel _playerModel;

        private void Awake()
        {
            // Obtener referencias a los otros componentes MVC
            if (_playerController == null) _playerController = GetComponent<PlayerController>();
            if (_playerModel == null) _playerModel = GetComponent<PlayerModel>();
        }

        private void Start()
        {
            InitializeSubViews();
            SubscribeToModelEvents();

            //Cursor Mouse
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            UnsubscribeFromModelEvents();
        }

        private void InitializeSubViews()
        {
            // Inicializar todas las sub-views
            animationView?.Initialize();
            weaponView?.Initialize();
            meleeView?.Initialize();
            audioView?.Initialize();
            effectsView?.Initialize();
        }

        private void SubscribeToModelEvents()
        {
            if (_playerModel == null)
            {
                Debug.LogWarning("PlayerModel is null, cannot subscribe to events");
                return;
            }

            // Suscribirse a eventos del modelo
            _playerModel.OnGameModeChanged += HandleGameModeChanged;
            _playerModel.OnMovementDirectionChanged += HandleMovementDirectionChanged;
            _playerModel.OnRawMovementDirectionChanged += HandleRawMovementDirectionChanged;
            _playerModel.OnAimDirectionChanged += HandleAimDirectionChanged;
            _playerModel.OnCanShootChanged += HandleCanShootChanged;

            // Forzar actualización inicial del modo de juego
            GameMode currentMode = _playerModel.CurrentGameMode;
            HandleGameModeChanged(currentMode);
        }

        private void UnsubscribeFromModelEvents()
        {
            if (_playerModel == null) return;

            // Desuscribirse de eventos del modelo
            _playerModel.OnGameModeChanged -= HandleGameModeChanged;
            _playerModel.OnMovementDirectionChanged -= HandleMovementDirectionChanged;
            _playerModel.OnRawMovementDirectionChanged -= HandleRawMovementDirectionChanged;
            _playerModel.OnAimDirectionChanged -= HandleAimDirectionChanged;
            _playerModel.OnCanShootChanged -= HandleCanShootChanged;
        }

        // Event Handlers - Responden a cambios en el modelo
        private void HandleGameModeChanged(GameMode newMode)
        {
            // Actualizar animaciones
            animationView?.SetGameMode(newMode);

            // Actualizar visibilidad del arma
            bool shouldShowWeapon = newMode != GameMode.Hub;
            weaponView?.SetWeaponVisibility(shouldShowWeapon);
            
            
        }

private void HandleMovementDirectionChanged(Vector3 moveDirection)
        {
            if (_playerModel == null) return;

            GameMode currentMode = _playerModel.CurrentGameMode;

            // Solo usar para rotación visual, NO para animaciones
            // Las animaciones ahora las maneja HandleRawMovementDirectionChanged
            if (currentMode == GameMode.Hub)
            {
                // En HUB: rotación simple hacia donde se mueve
                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    // Solo rotación, sin animación
                }
            }
            else
            {
                // En Combat: la rotación la maneja el aim, no el movimiento
                // No hacer nada aquí
            }
        }

        private void HandleAimDirectionChanged(Vector3 aimDirection)
        {
            if (_playerModel.CurrentGameMode == GameMode.Hub) return;

            // Solo actualizar aim en modo combat
            Vector3 mouseWorldPos = _playerController.MouseWorldPos;
            weaponView?.UpdateAiming(mouseWorldPos);
        }


        private void HandleRawMovementDirectionChanged(Vector3 rawMoveDirection)
        {
            if (_playerModel == null) return;

            GameMode currentMode = _playerModel.CurrentGameMode;

            if (currentMode == GameMode.Hub)
            {
                // En HUB: usar método principal UpdateMovement
                animationView?.UpdateMovement(rawMoveDirection, currentMode);
            }
            else
            {
                // En Combat: usar método específico con aim
                Vector3 aimDirection = _playerModel.AimDirection;
                animationView?.UpdateCombatMovementWithAim(rawMoveDirection, aimDirection);
            }
        }

        private void HandleCanShootChanged(bool canShoot)
        {
            // Lógica adicional si es necesaria cuando cambia la capacidad de disparo
            // Por ejemplo, cambiar UI indicators, etc.
        }

        // Public methods llamados por el PlayerModel para efectos
        public void PlayDamageEffect()
        {
            effectsView?.PlayDamageFlash();
            audioView?.PlayDamageSound();
        }

        public void PlayHealthEffect()
        {
            audioView?.PlayHealthSound();
        }

        // Backward compatibility - métodos que otros componentes podrían usar
        public bool CanUseWeapon()
        {
            return _playerModel?.CanShoot ?? false;
        }

        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }

        // Gizmos para debugging
        private void OnDrawGizmos()
        {
            if (_playerModel == null) return;

            // Mostrar información del estado actual
            Gizmos.color = _playerModel.CurrentGameMode == GameMode.Hub ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.5f);

            // Mostrar dirección de aim
            if (_playerModel.CurrentGameMode != GameMode.Hub)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _playerModel.AimDirection * 2f);
            }
        }
    }
}