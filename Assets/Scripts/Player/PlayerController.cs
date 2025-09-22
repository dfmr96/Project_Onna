using Core;
using Player.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const float AimRaycastMaxDistance = 100f;

        [SerializeField] private WeaponController weaponController = null;
        [SerializeField] private MeleeController meleeController = null;
        [SerializeField] private LayerMask groundLayer;

        [Header("Dash")] 
        private bool _isDashing = false;
        private float _dashEndTime = 0f;
        private float _lastDashTime = -Mathf.Infinity;
        private Vector3 _dashDirection;
        private const float DashDurationSeconds = 0.025f;
        private float _dashSpeed;
        [SerializeField] private ParticleSystem particleDash;

        private Rigidbody _rb;

        private PlayerInputHandler _playerInputHandler;
        private PlayerModel _playerModel;
        private PlayerInput _playerInput;
        private PlayerView _playerView;
        private Camera _mainCamera;
        private IInteractable currentInteractable;
        private bool canInteract = true;

        // Input data - se actualiza desde Input
        private Vector3 _inputDirection = Vector3.zero;
        private Vector3 _rawInputDirection = Vector3.zero;
        private Vector2 _rawAimInput = Vector2.zero;
        private Vector3 _mouseWorldPos;

        private bool _isReady = false;

        public float Speed => _playerInputHandler.MovementInput.magnitude;

        public Vector3 MouseWorldPos => _mouseWorldPos;



        [SerializeField] private float minAimDistance = 1f; // Distancia mínima desde el player
        private Vector3 _smoothedAimDir = Vector3.forward;
        private Vector3 _lastValidAimDir = Vector3.forward; // Última dirección válida del aim

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerInitializedSignal>(OnPlayerInitialized);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerInitializedSignal>(OnPlayerInitialized);
        }

        private void OnPlayerInitialized(PlayerInitializedSignal signal)
        {
            if (signal.Model != GetComponent<PlayerModel>()) return;

            _playerModel = signal.Model;
            _playerModel.InitializeGameMode();
            _isReady = true;
        }

        void Awake()
        {
            PlayerHelper.SetPlayer(gameObject);

            _mainCamera = Camera.main;
            _playerView = GetComponent<PlayerView>();
            _playerView.SetPlayerController(this);
            _playerInput = GetComponent<PlayerInput>();
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _playerInputHandler.DashPerformed += HandleDash;
            _rb = GetComponent<Rigidbody>();
            _playerInputHandler.InteractionPerformed += HandleInteraction;
            _playerInputHandler.FirePerformed += HandleFire;
            _playerInputHandler.MeleeAtackPerformed += HandleMelee;
            _playerInputHandler.ReloadPerformed += HandleReload;
        }

        private void Start()
        {
            if (particleDash != null)
            {
                particleDash.Stop();
            }
        }

        void Update()
        {
            if (!_isReady)
                return;

            // Capturar input
            _inputDirection = _playerInputHandler.MovementInput;
            _rawInputDirection = _playerInputHandler.RawMovementInput;
            _rawAimInput = _playerInputHandler.RawAimInput;

            // Procesar lógica de negocio - COMO QUE LOGICA DE NOGIOCIO EXPLIQUENME QUE CORNO ES ESTO!!! atte: sim!
            ProcessMovementLogic();
            ProcessAimingLogic();
        }

        private void FixedUpdate()
        {
            _rb.velocity = Vector3.zero;

            if (_isDashing)
            {
                if (Time.time > _dashEndTime)
                {
                    _isDashing = false;
                }
                else
                {
                    particleDash.Play();
                    Move(_dashDirection, _dashSpeed);
                    return;
                }
            }
            else particleDash.Stop();

            Move(_inputDirection, _playerModel.Speed);
        }


        //OLD MOVE2.0 METHOD WITH COLLISION DETECTION
        private void Move(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude <= 0.01f) return;

            Vector3 moveDir = direction.normalized;
            float moveDistance = speed * Time.fixedDeltaTime;

            if (Physics.CapsuleCast(_rb.position, _rb.position + Vector3.up * 1.8f, 0.4f, moveDir, out RaycastHit hit,
                    moveDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.isTrigger)
                {
                    Vector3 slideDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
                    _rb.MovePosition(_rb.position + slideDir * moveDistance);
                    return;
                }
            }

            _rb.MovePosition(_rb.position + moveDir * moveDistance);
        }

        //OLD MOVE METHOD WITH COLLISION DETECTION
        //private void Move(Vector3 direction, float speed)
        //{
        //    if (direction.sqrMagnitude <= 0.01f) return;

        //    Vector3 moveDir = direction.normalized;
        //    float moveDistance = speed * Time.fixedDeltaTime;
        //    Vector3 moveVector = moveDir * moveDistance;

        //    if (_rb.SweepTest(moveDir, out RaycastHit hit, moveDistance) && !hit.collider.isTrigger)
        //    {
        //        float adjustedDistance = Mathf.Max(hit.distance - 0.05f, 0f);
        //        Vector3 adjustedMove = moveDir * adjustedDistance;
        //        _rb.MovePosition(_rb.position + adjustedMove);
        //    }
        //    else _rb.MovePosition(_rb.position + moveVector);
        //}
        //private void Move(Vector3 direction, float speed)
        //{
        //    if (direction.sqrMagnitude > 0.01f)
        //    {
        //        Vector3 targetPosition = _rb.position + direction.normalized * (speed * Time.fixedDeltaTime);
        //        _rb.MovePosition(targetPosition);
        //    }
        //}

        private void Rotate(Vector3 aimDirection)
        {
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(aimDirection);
            }
        }

        private void HandleFire()
        {
            if (_playerModel.CanShoot)
            {
                weaponController.Attack();
            }
        }

        private void HandleMelee()
        {
            if (_playerModel.CanMelee) meleeController.Attack();
        }

        private void HandleReload()
        {
            if (_playerModel.CanShoot)
            {
                weaponController.Reloading();
            }
        }

        private void HandleInteraction()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f);

            IInteractable closestInteractable = null;

            foreach (var hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null && canInteract)
                {
                    interactable.Interact();
                    canInteract = false;
                }
            }

            currentInteractable = closestInteractable;
        }

        public void ToggleInteraction(bool value) => canInteract = value;

        private void HandleDash()
        {
            if (Time.time < _lastDashTime + _playerModel.DashCooldown || _inputDirection == Vector3.zero) return;

            _isDashing = true;
            _dashEndTime = Time.time + DashDurationSeconds;
            _dashDirection = _inputDirection.normalized;
            _lastDashTime = Time.time;

            _dashSpeed = _playerModel.DashDistance / DashDurationSeconds;
        }

        // MVC Methods - Lógica de negocio pura
        private void ProcessMovementLogic()
        {
            // Actualizar el modelo con la dirección de movimiento
            _playerModel.SetMovementDirection(_inputDirection);
            _playerModel.SetRawMovementDirection(_rawInputDirection);
            _playerModel.SetPosition(transform.position);
        }

        //private void ProcessAimingLogic()
        //{
        //    Vector3 aimDirection = CalculateAimDirection();
        //    _playerModel.SetAimDirection(aimDirection);

        //    // Actualizar capacidad de disparo basado en modo
        //    GameMode currentMode = GameModeSelector.SelectedMode;
        //    _playerModel.SetGameMode(currentMode);
        //    _playerModel.SetCanShoot(currentMode != GameMode.Hub);
        //    _playerModel.SetCanMelee(currentMode != GameMode.Hub);
        //}

        //private Vector3 CalculateAimDirection()
        //{
        //    if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        //    {
        //        Ray ray = _mainCamera.ScreenPointToRay(_rawAimInput);

        //        if (Physics.Raycast(ray, out RaycastHit hit, AimRaycastMaxDistance, groundLayer))
        //        {
        //            _mouseWorldPos = hit.point;
        //            _mouseWorldPos.y = 0;

        //            var position = transform.position;
        //            Vector3 flatPos = new Vector3(position.x, 0f, position.z);
        //            return (_mouseWorldPos - flatPos).normalized;
        //        }
        //    }
        //    else if (_playerInput.currentControlScheme == "Gamepad")
        //    {
        //        Vector3 aim = new Vector3(_rawAimInput.x, 0f, _rawAimInput.y);
        //        return Utils.IsoVectorConvert(aim).normalized;
        //    }

        //    return Vector3.forward;
        //}


        private void ProcessAimingLogic()
        {
            // Calcula la dirección hacia donde debe apuntar el jugador
            Vector3 targetDir = CalculateAimDirection();

            // Suaviza la dirección de aim entre frames para evitar vibración
            // Vector3.Lerp interpola entre la dirección anterior y la nueva
            _smoothedAimDir = Vector3.Lerp(_smoothedAimDir, targetDir, Time.deltaTime * 15f);

            // Actualiza la dirección de aim en el modelo del jugador
            _playerModel.SetAimDirection(_smoothedAimDir);

            // Actualizar capacidad de disparo
            GameMode currentMode = GameModeSelector.SelectedMode;
            _playerModel.SetGameMode(currentMode);
            _playerModel.SetCanShoot(currentMode != GameMode.Hub);
            _playerModel.SetCanMelee(currentMode != GameMode.Hub);
        }


        private Vector3 CalculateAimDirection()
        {
            if (_playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                // Crea un rayo desde la cámara hacia la posición del cursor
                Ray ray = _mainCamera.ScreenPointToRay(_rawAimInput);

                // Definimos un plano horizontal en Y=0 para calcular la posición de impacto
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                if (groundPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    _mouseWorldPos = hitPoint;
                    _mouseWorldPos.y = 0f;

                    Vector3 playerPos = new Vector3(transform.position.x, 0f, transform.position.z);
                    Vector3 aimDir = _mouseWorldPos - playerPos;

                    // Si el cursor está muy cerca, mantener la última dirección válida
                    if (aimDir.magnitude < minAimDistance)
                    {
                        // Si está fuera de la distancia mínima, actualizamos la última dirección válida
                        aimDir = _lastValidAimDir;
                    }
                    else
                    {
                        // Devolver la dirección normalizada lista para usar
                        _lastValidAimDir = aimDir.normalized;
                    }

                    // Valor por defecto si no hay input válido
                    return aimDir.normalized;
                }
            }
            else if (_playerInput.currentControlScheme == "Gamepad")
            {
                Vector3 aim = new Vector3(_rawAimInput.x, 0f, _rawAimInput.y);
                return Utils.IsoVectorConvert(aim).normalized;
            }

            return Vector3.forward;
        }





        private void OnDrawGizmos() => Gizmos.DrawSphere(MouseWorldPos, 0.5f);
    }
}