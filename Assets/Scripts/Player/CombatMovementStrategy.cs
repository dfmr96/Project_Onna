using UnityEngine;

[System.Serializable]
public class CombatMovementStrategy : BaseMovementStrategy
{
    [Header("Combat Settings")]
    [SerializeField] private bool enableAiming = true;
    [SerializeField] private bool enableMouseDetection = true;
    [SerializeField] private bool useUpperLayerAnimator = true;
    [SerializeField] private bool useLowerLayerAnimator = true;

    [SerializeField] private PlayerRigController rigController;

    public override void Initialize(Transform playerTransform, InputVisualizerGizmos inputVisualizer)
    {
        base.Initialize(playerTransform, inputVisualizer);

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
        HandleAiming();
        HandleMouseDetection();
        HandleAnimatorLayers();
    }

    protected override void HandleMovement()
    {
        Vector3 cameraRelativeInput;

        if (inputVisualizer != null)
        {
            cameraRelativeInput = inputVisualizer.cameraRelativeInput;
        }
        else
        {
            cameraRelativeInput = Vector3.zero;
        }

        Vector3 moveDirection;

        if (cameraRelativeInput.magnitude > 0.1f)
        {
            moveDirection = cameraRelativeInput;
            lastInputDirection = cameraRelativeInput.normalized;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        Vector3 targetVelocity = moveDirection * moveSpeed;

        float smoothSpeed = targetVelocity.magnitude > 0.1f ? acceleration : deceleration;
        smoothedMovement = Vector3.MoveTowards(smoothedMovement, targetVelocity, smoothSpeed * Time.deltaTime);

        currentMoveInput = smoothedMovement;
    }

    protected override void MoveCharacter()
    {
        if (currentMoveInput.magnitude < 0.01f) return;

        Vector3 desiredPosition = playerTransform.position + currentMoveInput * Time.deltaTime;

        if (useNavMeshValidation)
        {
            Vector3 validatedPosition = ValidatePositionWithNavMesh(desiredPosition);
            playerTransform.position = validatedPosition;
        }
        else
        {
            playerTransform.position = desiredPosition;
        }

        lastValidPosition = playerTransform.position;
    }

    protected override void CalculateAnimationInput()
    {
        if (inputVisualizer == null)
        {
            animationMovementInput = Vector2.zero;
            return;
        }

        // Combat mode: Use mouse relative movement for strafe/backpedal animations
        if (mouseAiming == null || mouseAiming.aimTarget == null)
        {
            // Fall back to camera relative if no mouse aiming
            CalculateCameraRelativeInput();
            return;
        }

        // Get camera-relative input from InputVisualizerGizmos
        Vector3 cameraRelativeWorldInput = inputVisualizer.cameraRelativeInput;

        // If no input, return zero
        if (cameraRelativeWorldInput.magnitude < 0.1f)
        {
            animationMovementInput = Vector2.zero;
            return;
        }

        // Get direction from player to mouse target (aim target)
        Vector3 playerPosition = playerTransform.position;
        Vector3 mouseTargetPosition = mouseAiming.aimTarget.position;

        Vector3 toMouseDirection = (mouseTargetPosition - playerPosition);
        toMouseDirection.y = 0; // Project to horizontal plane
        toMouseDirection.Normalize();

        // If no mouse direction, fall back to camera-relative input
        if (toMouseDirection.magnitude < 0.1f)
        {
            animationMovementInput = new Vector2(cameraRelativeWorldInput.x, cameraRelativeWorldInput.z);
            return;
        }

        // Calculate right vector perpendicular to mouse direction
        Vector3 mouseRight = new Vector3(toMouseDirection.z, 0, -toMouseDirection.x);

        // Project camera-relative world input onto mouse direction vectors
        float forwardComponent = Vector3.Dot(cameraRelativeWorldInput, toMouseDirection);
        float rightComponent = Vector3.Dot(cameraRelativeWorldInput, mouseRight);

        animationMovementInput = new Vector2(rightComponent, forwardComponent);

        if (showAnimationDebugInfo)
        {
            Debug.Log($"[COMBAT] Camera Input: {cameraRelativeWorldInput} | Mouse Dir: {toMouseDirection} | Animation Input: {animationMovementInput}");
        }
    }

    private void CalculateCameraRelativeInput()
    {
        // Simple camera-relative mapping as fallback
        Vector3 cameraRelativeWorldInput = inputVisualizer.cameraRelativeInput;
        animationMovementInput = new Vector2(cameraRelativeWorldInput.x, cameraRelativeWorldInput.z);
    }

    private void HandleAiming()
    {
        if (!enableAiming) return;
        
        // TODO: Implement aiming logic
        // This will handle aim input, crosshair positioning, etc.
    }

    private void HandleMouseDetection()
    {
        if (!enableMouseDetection) return;
        
        // TODO: Implement mouse detection logic
        // This will handle mouse movement for camera/aim control
    }

    private void HandleAnimatorLayers()
    {
        if (animator == null) return;

        // Handle Lower Layer (legs/movement)
        if (useLowerLayerAnimator)
        {
            HandleLowerLayerAnimation();
        }

        // Handle Upper Layer (arms/torso for aiming)
        if (useUpperLayerAnimator)
        {
            HandleUpperLayerAnimation();
        }
    }

    private void HandleLowerLayerAnimation()
    {
        // TODO: Set movement parameters for lower body
        // Examples:
        // animator.SetFloat("Speed", currentMoveInput.magnitude);
        // animator.SetFloat("VelocityX", currentMoveInput.x);
        // animator.SetFloat("VelocityZ", currentMoveInput.z);
        // animator.SetBool("IsMoving", currentMoveInput.magnitude > 0.1f);
    }

    private void HandleUpperLayerAnimation()
    {
        // TODO: Set aiming/combat parameters for upper body
        // Examples:
        // animator.SetBool("IsAiming", isAiming);
        // animator.SetFloat("AimHorizontal", aimInput.x);
        // animator.SetFloat("AimVertical", aimInput.y);
    }

    // Public setters for external systems to configure combat behavior
    public void SetAimingEnabled(bool enabled) => enableAiming = enabled;
    public void SetMouseDetectionEnabled(bool enabled) => enableMouseDetection = enabled;
    public void SetUpperLayerEnabled(bool enabled) => useUpperLayerAnimator = enabled;
    public void SetLowerLayerEnabled(bool enabled) => useLowerLayerAnimator = enabled;

    // Getters for debugging/external systems
    public bool IsAimingEnabled() => enableAiming;
    public bool IsMouseDetectionEnabled() => enableMouseDetection;
    public bool IsUpperLayerEnabled() => useUpperLayerAnimator;
    public bool IsLowerLayerEnabled() => useLowerLayerAnimator;
}