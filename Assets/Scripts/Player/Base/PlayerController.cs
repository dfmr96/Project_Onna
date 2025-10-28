using Core;
using Player.Melee;
using Player.Movement;
using Player.Weapon;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public Action HandlePauseAccess;
        
        [SerializeField] private WeaponController weaponController = null;
        [SerializeField] private MeleeController meleeController = null;
        [SerializeField] private DashController dashController = null;
        
        private PlayerInputHandler _playerInputHandler;
        private PlayerModel _playerModel;
        private PlayerView _playerView;
        private IInteractable currentInteractable;
        private bool canInteract = true;

        

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerInitializedSignal>(OnPlayerInitialized);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerInitializedSignal>(OnPlayerInitialized);
            
            if (_playerInputHandler != null)
            {
                _playerInputHandler.InteractionPerformed -= HandleInteraction;
                _playerInputHandler.FirePerformed -= HandleFire;
                _playerInputHandler.MeleeAtackPerformed -= HandleMelee;
                _playerInputHandler.ReloadPerformed -= HandleReload;
                _playerInputHandler.DashPerformed -= HandleDash;
                _playerInputHandler.OnPauseGame -= HandlePause;
            }
        }

        private void OnPlayerInitialized(PlayerInitializedSignal signal)
        {
            if (signal.Model != GetComponent<PlayerModel>()) return;

            _playerModel = signal.Model;
            _playerModel.InitializeGameMode();
        }

        void Awake()
        {
            PlayerHelper.SetPlayer(gameObject);

            _playerView = GetComponent<PlayerView>();
            _playerView.SetPlayerController(this);
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _playerInputHandler.InteractionPerformed += HandleInteraction;
            _playerInputHandler.FirePerformed += HandleFire;
            _playerInputHandler.MeleeAtackPerformed += HandleMelee;
            _playerInputHandler.ReloadPerformed += HandleReload;
            _playerInputHandler.DashPerformed += HandleDash;
            _playerInputHandler.OnPauseGame += HandlePause;

        }
        private void HandleFire()
        {
            if (weaponController.IsSkillCheckActive())
                weaponController.TrySkillCheck();

            else if (_playerModel != null && _playerModel.CanShoot)
                weaponController.Attack();
        }

        private void HandleMelee()
        {
            if (_playerModel != null && _playerModel.CanMelee) meleeController.Attack();
        }
        private void HandleReload()
        {
            if (weaponController.IsSkillCheckActive())
                weaponController.TrySkillCheck();

            else if (_playerModel != null && _playerModel.CanShoot)
                weaponController.Reloading();
        }
        
        private void HandleDash()
        {
            if (dashController != null) dashController.TryDash();
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

        private void HandlePause() => HandlePauseAccess?.Invoke();
        public void ToggleInteraction(bool value) => canInteract = value;
    }
}