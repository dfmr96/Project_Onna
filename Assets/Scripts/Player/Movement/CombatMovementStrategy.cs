using UnityEngine;

namespace Player.Movement
{
    [System.Serializable]
    public class CombatMovementStrategy : BaseMovementStrategy
    {
        [Header("Combat Settings")]
        [SerializeField] private bool enableMouseDetection = true;
        [SerializeField] private bool useUpperLayerAnimator = true;
        [SerializeField] private bool useLowerLayerAnimator = true;

        [Header("Animator Layer Settings")]
        [SerializeField] private string upperLayerName = "UpperBody";
        [SerializeField] private string lowerLayerName = "LowerBody";

        [SerializeField] private PlayerRigController rigController;

        private int upperLayerIndex = -1;
        private int lowerLayerIndex = -1;

        public override void Initialize(Transform playerTransform, PlayerModel playerModel)
        {
            base.Initialize(playerTransform, playerModel);

            // Get layer indices for animator layer control
            if (animator != null)
            {
                upperLayerIndex = animator.GetLayerIndex(upperLayerName);
                lowerLayerIndex = animator.GetLayerIndex(lowerLayerName);

                if (upperLayerIndex == -1)
                    Debug.LogWarning($"CombatMovementStrategy: UpperLayer '{upperLayerName}' not found in Animator!");

                if (lowerLayerIndex == -1)
                    Debug.LogWarning($"CombatMovementStrategy: LowerLayer '{lowerLayerName}' not found in Animator!");

                // Set layer weights for Combat mode (all layers active)
                SetCombatLayerWeights();
            }

            if (rigController == null)
            {
                Debug.LogWarning("CombatMovementStrategy: No PlayerRigController component found on player!");
            }
            else
            {
                // Set initial combat state (pistol aim)
                rigController.SetPistolAimState();
            }

            // Ensure weapon is visible in Combat mode
            if (weaponView != null)
            {
                weaponView.SetWeaponVisibility(true);
            }
        }

        protected override void HandleStrategySpecificLogic()
        {
        }

        private void SetCombatLayerWeights()
        {
            if (animator == null) return;

            // Set UpperLayer and LowerLayer weights to 1 (all layers active in combat)
            if (upperLayerIndex != -1)
            {
                animator.SetLayerWeight(upperLayerIndex, 1f);
            }

            if (lowerLayerIndex != -1)
            {
                animator.SetLayerWeight(lowerLayerIndex, 1f);
            }

            // Base Layer (index 0) stays at weight 1
            animator.SetLayerWeight(0, 1f);
        }

        protected override void HandleMovement()
        {
            Vector3 cameraRelativeInput = inputService != null
                ? inputService.CameraRelativeInput
                : Vector3.zero;

            Vector3 moveDirection = cameraRelativeInput.magnitude > 0.1f
                ? cameraRelativeInput
                : Vector3.zero;

            if (moveDirection.sqrMagnitude > 0)
                lastInputDirection = moveDirection.normalized;

            Vector3 targetVelocity = moveDirection * Speed;
            float smoothSpeed = targetVelocity.magnitude > 0.1f ? acceleration : deceleration;

            smoothedMovement = Vector3.MoveTowards(smoothedMovement, targetVelocity, smoothSpeed * Time.deltaTime);
            currentMoveInput = smoothedMovement;
        }

        protected override void MoveCharacter()
        {
            if (currentMoveInput.magnitude < 0.01f) return;
            if (playerRigidbody == null) return;
            //if (playerMovement.IsDashing) return;

            Vector3 desiredPosition = playerRigidbody.position + currentMoveInput * Time.deltaTime;

            if (useNavMeshValidation)
            {
                Vector3 validatedPosition = ValidatePositionWithNavMesh(desiredPosition);
                playerRigidbody.MovePosition(validatedPosition);
                lastValidPosition = validatedPosition;
            }
            else
            {
                playerRigidbody.MovePosition(desiredPosition);
                lastValidPosition = desiredPosition;
            }
        }

        protected override void CalculateAnimationInput()
        {
            if (inputService == null)
            {
                animationMovementInput = Vector2.zero;
                return;
            }

            if (enableMouseDetection && mouseAiming != null && mouseAiming.aimTarget != null)
            {
                Vector3 toMouse = (mouseAiming.aimTarget.position - playerTransform.position);
                toMouse.y = 0;

                if (toMouse.sqrMagnitude > 0.01f)
                {
                    toMouse.Normalize();
                    Vector3 right = new Vector3(toMouse.z, 0, -toMouse.x);
                    Vector3 cameraInput = inputService.CameraRelativeInput;

                    float forward = Vector3.Dot(cameraInput, toMouse);
                    float rightComp = Vector3.Dot(cameraInput, right);
                    animationMovementInput = new Vector2(rightComp, forward);
                    return;
                }
            }

            // Fallback to camera-relative input
            Vector3 cameraRelativeInput = inputService.CameraRelativeInput;
            animationMovementInput = new Vector2(cameraRelativeInput.x, cameraRelativeInput.z);
        }

        // Public setters for external systems to configure combat behavior
        public void SetMouseDetectionEnabled(bool enabled) => enableMouseDetection = enabled;
        public void SetUpperLayerEnabled(bool enabled) => useUpperLayerAnimator = enabled;
        public void SetLowerLayerEnabled(bool enabled) => useLowerLayerAnimator = enabled;

        // Getters for debugging/external systems
        public bool IsMouseDetectionEnabled() => enableMouseDetection;
        public bool IsUpperLayerEnabled() => useUpperLayerAnimator;
        public bool IsLowerLayerEnabled() => useLowerLayerAnimator;
    }
}