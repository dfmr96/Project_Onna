using System;
using Core;
using Player.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private const float AimRaycastMaxDistance = 100f;
    
        [SerializeField] private WeaponController weaponController = null;
        [SerializeField] private LayerMask groundLayer;

        [Header("Dash")]
        private bool _isDashing = false;
        private float _dashEndTime = 0f;
        private float _lastDashTime = -Mathf.Infinity;
        private Vector3 _dashDirection;
        private const float DashDurationSeconds = 0.2f;

        private CharacterController _characterController;
        private Vector3 _aimDirection = Vector3.forward;
        private PlayerInputHandler _playerInputHandler;
        private Vector3 _direction = Vector3.zero;
        private PlayerModel _playerModel;
        private PlayerInput _playerInput;
        private PlayerView _playerView;
        private Vector3 _mouseWorldPos;
        private Camera _mainCamera;
        private IInteractable currentInteractable;
        private bool canInteract = true;

        private bool _isReady = false;

        public float Speed => _playerInputHandler.MovementInput.magnitude;

        public Vector3 MouseWorldPos => _mouseWorldPos;

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

//            Debug.Log("🎯 PlayerController: recibida señal PlayerInitializedSignal");

            _playerModel = signal.Model;
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
            _characterController = GetComponent<CharacterController>();

            _playerInputHandler.InteractionPerformed += HandleInteraction;
            _playerInputHandler.FirePerformed += HandleFire;
        }

        void Update()
        {
            if (!_isReady)
            {
                Debug.Log("🕒 PlayerController: esperando inicialización...");
                return;
            }

            _direction = _playerInputHandler.MovementInput;
            HandleAiming(_playerInputHandler.RawAimInput);

            if (_isDashing)
            {
                if (Time.time > _dashEndTime)
                {
                    _isDashing = false;
                }
                else
                {
                    Move(_dashDirection, _playerModel.DashDistance);
                    return;
                }
            }

            Move(_direction, _playerModel.Speed);
        }

        private void HandleAiming(Vector2 rawInput)
        {
            if (_playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                Ray ray = _mainCamera.ScreenPointToRay(rawInput);

                if (Physics.Raycast(ray, out RaycastHit hit, AimRaycastMaxDistance, groundLayer))
                {
                    _mouseWorldPos = hit.point;
                    _mouseWorldPos.y = 0;

                    var position = transform.position;
                    Vector3 flatPos = new Vector3(position.x, 0f, position.z);
                    _aimDirection = (MouseWorldPos - flatPos).normalized;
                }
            }
            else if (_playerInput.currentControlScheme == "Gamepad")
            {
                Vector3 aim = new Vector3(rawInput.x, 0f, rawInput.y);
                _aimDirection = Utils.IsoVectorConvert(aim).normalized;
            }
        }

        private void Move(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                _characterController.Move(direction * (speed * Time.deltaTime));
            }
        }

        private void Rotate(Vector3 aimDirection)
        {
            if (aimDirection.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(aimDirection);
            }
        }
    
        private void HandleFire()
        {
            weaponController.Attack();
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

        public void ToggleInteraction(bool value) { canInteract = value; }

        private void HandleDash()
        {
            if (Time.time < _lastDashTime + _playerModel.DashCooldown || _direction == Vector3.zero) return;

            _isDashing = true;
            _dashEndTime = Time.time + DashDurationSeconds;
            _dashDirection = _direction.normalized;
            _lastDashTime = Time.time;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(MouseWorldPos, 0.5f);
        }
    }
}