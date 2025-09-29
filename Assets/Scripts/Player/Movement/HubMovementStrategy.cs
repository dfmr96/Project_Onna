using Player;
using Player.Movement;
using Services;
using UnityEngine;

[System.Serializable]
public class HubMovementStrategy : BaseMovementStrategy
{

    [Header("Hub Settings")]
    [SerializeField] private bool useLowerLayerAnimator = true;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    [SerializeField] private bool enableRotation = true;

    [Header("Weapon Settings")]
    [SerializeField] private bool hideWeaponInHub = true;

    [Header("Animator Layer Settings")]
    [SerializeField] private string upperLayerName = "UpperBody";
    [SerializeField] private string lowerLayerName = "LowerBody";

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
                Debug.LogWarning($"HubMovementStrategy: UpperLayer '{upperLayerName}' not found in Animator!");

            if (lowerLayerIndex == -1)
                Debug.LogWarning($"HubMovementStrategy: LowerLayer '{lowerLayerName}' not found in Animator!");

            // Set layer weights for Hub mode (only Base Layer active)
            SetHubLayerWeights();
        }

        // Configure weapon visibility for Hub mode
        SetWeaponVisibility();
    }


    protected override void HandleStrategySpecificLogic()
    {
        HandlePlayerRotation();
    }

    private void SetHubLayerWeights()
    {
        if (animator == null) return;

        // Set UpperLayer and LowerLayer weights to 0 (only Base Layer active)
        if (upperLayerIndex != -1)
        {
            animator.SetLayerWeight(upperLayerIndex, 0f);
        }

        if (lowerLayerIndex != -1)
        {
            animator.SetLayerWeight(lowerLayerIndex, 0f);
        }

        // Base Layer (index 0) is always weight 1 by default
        // But we can ensure it explicitly
        animator.SetLayerWeight(0, 1f);
    }

    private void SetWeaponVisibility()
    {
        if (weaponView == null) return;

        // Hide weapon in Hub mode if setting is enabled
        bool showWeapon = !hideWeaponInHub;
        weaponView.SetWeaponVisibility(showWeapon);

        if (showAnimationDebugInfo)
        {
            Debug.Log($"[HUB] Weapon visibility set to: {showWeapon}");
        }
    }

    private void HandlePlayerRotation()
    {
        if (!enableRotation || playerTransform == null) return;

        // Use the movement input direction for rotation (not the smoothed movement)
        Vector3 movementDirection = Vector3.zero;

        if (inputService != null)
        {
            Vector3 cameraRelativeInput = inputService.CameraRelativeInput;

            if (cameraRelativeInput.magnitude > 0.1f)
            {
                // Use the raw input direction for immediate rotation response
                movementDirection = cameraRelativeInput.normalized;
            }
        }

        // Only rotate if there's movement input
        if (movementDirection.magnitude > 0.1f)
        {
            // Calculate target rotation towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Smoothly rotate towards target
            playerTransform.rotation = Quaternion.RotateTowards(
                playerTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    protected override void HandleMovement()
    {
        Vector3 cameraRelativeInput;

        if (inputService != null)
        {
            cameraRelativeInput = inputService.CameraRelativeInput;
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

        // Hub mode: Simpler movement, less strict NavMesh validation
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
        if (inputService == null)
        {
            animationMovementInput = Vector2.zero;
            return;
        }

        // Hub mode: Use simple camera-relative movement (no mouse relative calculation)
        Vector3 cameraRelativeWorldInput = inputService.CameraRelativeInput;

        if (cameraRelativeWorldInput.magnitude < 0.1f)
        {
            animationMovementInput = Vector2.zero;
            return;
        }

        // Simple camera-relative mapping: X and Z to animation X and Y
        animationMovementInput = new Vector2(cameraRelativeWorldInput.x, cameraRelativeWorldInput.z);

        if (showAnimationDebugInfo)
        {
            Debug.Log($"[HUB] Camera Input: {cameraRelativeWorldInput} | Animation Input: {animationMovementInput}");
        }
    }
}