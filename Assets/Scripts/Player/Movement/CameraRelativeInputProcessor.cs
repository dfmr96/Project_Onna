using Services;
using UnityEngine;
using VContainer.Unity;

namespace Player.Movement
{
    public class CameraRelativeInputProcessor : IInputService, ITickable
    {
        private readonly Camera cameraReference;
        private readonly PlayerInputHandler inputHandler;

        public Vector3 CameraRelativeInput { get; private set; }
        public Vector2 RawInput { get; private set; }

        protected CameraRelativeInputProcessor(Camera camera, PlayerInputHandler inputHandler)
        {
            this.cameraReference = camera;
            this.inputHandler = inputHandler;

            if (this.cameraReference == null)
            {
                Debug.LogWarning("CameraRelativeInputProcessor: No Camera provided!");
            }

            if (this.inputHandler == null)
            {
                Debug.LogWarning("CameraRelativeInputProcessor: No PlayerInputHandler provided!");
            }
        }

        public void Tick()
        {
            CalculateCameraRelativeInput();
        }

        private void CalculateCameraRelativeInput()
        {
            if (inputHandler == null)
            {
                CameraRelativeInput = Vector3.zero;
                RawInput = Vector2.zero;
                return;
            }

            // Get raw input from PlayerInputHandler
            Vector3 rawMovementInput = inputHandler.RawMovementInput;
            RawInput = new Vector2(rawMovementInput.x, rawMovementInput.z);

            if (cameraReference == null)
            {
                // Fallback: no camera conversion
                CameraRelativeInput = rawMovementInput;
                return;
            }

            // Get camera forward and right vectors (projected on XZ plane)
            Vector3 cameraForward = cameraReference.transform.forward;
            Vector3 cameraRight = cameraReference.transform.right;

            // Project camera vectors onto horizontal plane (remove Y component)
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate camera-relative movement direction
            CameraRelativeInput = (cameraForward * RawInput.y + cameraRight * RawInput.x);
        }

        // Public getters for compatibility
        public Vector2 GetCurrentInput() => RawInput;
        public Vector3 GetCameraRelativeInput() => CameraRelativeInput;

        // For debugging
        public bool HasInput() => RawInput.magnitude > 0.1f;
    }
}