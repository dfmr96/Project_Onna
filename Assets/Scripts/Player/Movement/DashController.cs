using System;
using Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UnityEngine.AI;

namespace Player.Movement
{
    public class DashController : MonoBehaviour
    {
        [Header("Dash Settings")]
        [SerializeField] private float dashDistance = 5f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private AnimationCurve dashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public event Action<Vector3,Transform> OnDashStarted;
        public event Action<Vector3> OnDashEnded;

        private bool isDashing;
        private float lastDashTime;
        private Vector3 dashDirection;
        private IInputService inputService;
        [SerializeField] PlayerMovement playerMovement;
        [SerializeField] Transform playerTransform;
        private Rigidbody playerRigidbody;

        public bool CanDash => !isDashing &&
                              Time.time - lastDashTime >= dashCooldown &&
                              IsInCombatMode();

        private void Start()
        {
            // Get Rigidbody component
            playerRigidbody = GetComponent<Rigidbody>();
            if (playerRigidbody == null)
            {
                Debug.LogError("DashController: No Rigidbody component found on player!");
            }

            // Get input service from VContainer
            var lifetimeScope = GetComponentInParent<LifetimeScope>();
            if (lifetimeScope != null)
            {
                inputService = lifetimeScope.Container.Resolve<IInputService>();
            }
            else
            {
                Debug.LogError("DashController: No LifetimeScope found! Make sure PlayerLifetimeScope is configured on player or parent GameObject.");
            }

            // Get PlayerMovement component to check current strategy
            if (playerMovement == null)
            {
                Debug.LogError("DashController: No PlayerMovement component found on the same GameObject!");
            }
        }

        private bool IsInCombatMode()
        {
            return playerMovement != null &&
                   playerMovement.GetCurrentStrategyType() == PlayerMovement.MovementStrategyType.Combat;
        }

        public void TryDash()
        {
            if (!CanDash) return;

            // Use camera-relative input direction, fallback to forward if no input
            Vector3 inputDirection = inputService?.CameraRelativeInput ?? Vector3.zero;
            dashDirection = inputDirection.sqrMagnitude > 0 ? inputDirection.normalized : transform.forward;

            StartCoroutine(DashRoutine());
        }

        private System.Collections.IEnumerator DashRoutine()
        {
            if (playerRigidbody == null) yield break;

            Debug.Log("Dash started");
            isDashing = true;
            lastDashTime = Time.time;

            Vector3 start = playerRigidbody.position;
            Vector3 targetEnd = start + dashDirection * dashDistance;
            OnDashStarted?.Invoke(start, playerTransform);

            float t = 0f;
            while (t < dashDuration)
            {
                t += Time.deltaTime;
                float curveValue = dashCurve.Evaluate(t / dashDuration);
                Vector3 desiredPosition = Vector3.Lerp(start, targetEnd, curveValue);

                // Progressive NavMesh validation
                Vector3 validatedPosition = ValidatePositionProgressive(desiredPosition);

                // If validation failed (returned current position), stop the dash
                if (validatedPosition == playerRigidbody.position &&
                    Vector3.Distance(desiredPosition, playerRigidbody.position) > 0.1f)
                {
                    Debug.Log("Dash stopped due to NavMesh obstruction");
                    break;
                }

                playerRigidbody.MovePosition(validatedPosition);
                yield return null;
            }

            isDashing = false;
            Debug.Log("Dash ended");
            OnDashEnded?.Invoke(playerRigidbody.position);
        }

        private Vector3 ValidatePositionProgressive(Vector3 desiredPosition)
        {
            if (playerRigidbody == null) return desiredPosition;

            NavMeshHit hit;

            // Try to find a valid position near the desired position
            if (NavMesh.SamplePosition(desiredPosition, out hit, 0.5f, NavMesh.AllAreas))
            {
                // Check if the hit position is reasonably close to desired position
                float distance = Vector3.Distance(desiredPosition, hit.position);

                // If the valid position is too far from desired, it means we hit an obstacle
                if (distance > 1f)
                {
                    return playerRigidbody.position; // Stop movement
                }

                return hit.position; // Valid position found
            }

            // No valid position found - stop the dash
            return playerRigidbody.position;
        }
    }
}