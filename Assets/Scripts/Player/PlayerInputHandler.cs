using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector3 RawMovementInput { get; private set; }
        public Vector2 RawAimInput { get; private set; }

        public static PlayerInputHandler Instance { get; private set; }

        public event Action FirePerformed;
        public event Action MeleeAtackPerformed;
        public event Action ReloadPerformed;
        public event Action DashPerformed;
        public event Action InteractionPerformed;
        public event Action OnPauseGame;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            RawMovementInput = new Vector3(input.x, 0, input.y);
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            RawAimInput = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed) FirePerformed?.Invoke();
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed) ReloadPerformed?.Invoke();
        }

        public void OnMeleeAtack(InputAction.CallbackContext context)
        {
            if (context.performed) MeleeAtackPerformed?.Invoke();
        }


        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) InteractionPerformed?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DashPerformed?.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed) OnPauseGame?.Invoke();
        }
    }
}