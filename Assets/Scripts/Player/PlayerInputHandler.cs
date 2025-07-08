using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector3 MovementInput { get; private set; }
        public Vector2 RawAimInput { get; private set; }

        public event Action FirePerformed;

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            MovementInput = Utils.IsoVectorConvert(new Vector3(input.x, 0, input.y)).normalized;
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            RawAimInput = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                FirePerformed?.Invoke();
            }
        }
    }
}