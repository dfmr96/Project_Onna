using Services;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Player.Movement
{
    public abstract class BaseMovementStrategy : IMovementStrategy
    {
        [Header("Movement Settings")]
        [SerializeField] protected PlayerModel playerModel;

        protected float Speed
        {
            get
            {
                var value = playerModel?.Speed ?? 0;
                speed = value;
                return value;
            }
        }

        [SerializeField] protected float speed = 0;
        [SerializeField] protected float acceleration = 10f;
        [SerializeField] protected float deceleration = 15f;

        [Header("NavMesh Validation Settings")]
        [SerializeField] protected bool useNavMeshValidation = true;
        [SerializeField] protected float navMeshSampleDistance = 1f;
        [SerializeField] protected float maxNavMeshDistance = 2f;
        [SerializeField] protected float playerHeightOffset = 1f;

        [Header("Debug Visualization")]
        [SerializeField] protected float gizmoLookAheadTime = 0.5f;

        protected Transform playerTransform;
        protected Rigidbody playerRigidbody;
        protected IInputService inputService;

        protected Vector3 currentMoveInput;
        protected Vector3 smoothedMovement;
        protected Vector3 lastInputDirection;
        protected Vector3 lastValidPosition;

        // Animation components
        [SerializeField] protected Animator animator;
        protected MouseGroundAiming mouseAiming;
        protected Vector2 animationMovementInput;

        // Weapon components
        [SerializeField] protected PlayerWeaponView weaponView;

        [Header("Debug Animation")]
        [SerializeField] protected bool showAnimationDebugInfo = false;
        
        

        public virtual void Initialize(Transform playerTransform, PlayerModel playerModel)
        {
            this.playerTransform = playerTransform;
            this.playerModel = playerModel;
            this.playerRigidbody = playerTransform.GetComponent<Rigidbody>();

            if (playerRigidbody == null)
            {
                Debug.LogError("BaseMovementStrategy: No Rigidbody component found on player!");
            }

            //this.speed = speed;
            // Get input service from VContainer
            var lifetimeScope = playerTransform.GetComponentInParent<LifetimeScope>();
            if (lifetimeScope != null)
            {
                this.inputService = lifetimeScope.Container.Resolve<IInputService>();
            }
            else
            {
                Debug.LogError("BaseMovementStrategy: No LifetimeScope found! Make sure PlayerLifetimeScope is configured on player or parent GameObject.");
            }

            // Initialize animation components
            if (animator == null)
            {
                Debug.LogWarning("BaseMovementStrategy: No Animator component found on player!");
            }

            mouseAiming = Object.FindObjectOfType<MouseGroundAiming>();
            if (mouseAiming == null)
            {
                Debug.LogWarning("BaseMovementStrategy: No MouseGroundAiming component found!");
            }

            if (weaponView == null)
            {
                Debug.LogWarning("BaseMovementStrategy: No PlayerWeaponView component found!");
            }

            lastValidPosition = playerTransform.position;

            if (useNavMeshValidation)
            {
                NavMeshHit hit;
                Vector3 checkPosition = new Vector3(playerTransform.position.x, playerTransform.position.y - playerHeightOffset, playerTransform.position.z);

                if (NavMesh.SamplePosition(checkPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                {
                    Vector3 correctedPosition = new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
                    playerTransform.position = correctedPosition;
                    lastValidPosition = correctedPosition;
                }
                else
                {
                    Debug.LogWarning("BaseMovementStrategy: Starting position is not on NavMesh! Make sure there's a baked NavMesh in the scene or disable NavMesh validation.");
                    lastValidPosition = playerTransform.position;
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void Update()
        {
            HandleMovement();
            MoveCharacter();
            CalculateAnimationInput();
            UpdateAnimatorParameters();
            HandleStrategySpecificLogic();
        }

        protected abstract void HandleStrategySpecificLogic();
        protected abstract void CalculateAnimationInput();
        protected abstract void HandleMovement();
        protected abstract void MoveCharacter();

        protected virtual void UpdateAnimatorParameters()
        {
            if (animator == null) return;

            // Update animator parameters with movement input (rounded to integers)
            animator.SetFloat("moveX", Mathf.Round(animationMovementInput.x));
            animator.SetFloat("moveY", Mathf.Round(animationMovementInput.y));

            // Optional: Add movement magnitude parameter
            float movementMagnitude = animationMovementInput.magnitude;
            animator.SetFloat("movementSpeed", movementMagnitude);

            // Optional: Add boolean for any movement
            bool isMoving = movementMagnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);

            if (showAnimationDebugInfo)
            {
                Debug.Log($"[{this.GetType().Name}] Animation Input: {animationMovementInput} | Magnitude: {movementMagnitude:F2}");
            }
        }

        // Utility method for child strategies to use
        protected Vector3 ValidatePositionWithNavMesh(Vector3 desiredPosition)
        {
            NavMeshHit hit;

            Vector3 navMeshCheckPosition = new Vector3(desiredPosition.x, desiredPosition.y - playerHeightOffset, desiredPosition.z);

            if (NavMesh.SamplePosition(navMeshCheckPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                Vector3 desiredXZ = new Vector3(desiredPosition.x, 0, desiredPosition.z);
                Vector3 hitXZ = new Vector3(hit.position.x, 0, hit.position.z);
                float distance = Vector3.Distance(desiredXZ, hitXZ);

                if (distance <= 0.5f)
                {
                    return new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
                }
            }

            return playerTransform.position;
        }

        public virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || playerTransform == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(playerTransform.position + Vector3.up, currentMoveInput);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(playerTransform.position + Vector3.up * 1.2f, smoothedMovement);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerTransform.position + Vector3.up * 1.4f, lastInputDirection * 2f);

            if (useNavMeshValidation)
            {
                Vector3 feetPosition = GetFeetPosition();

                float distanceToLastValid = Vector3.Distance(playerTransform.position, lastValidPosition);
                if (distanceToLastValid > 0.5f)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z), 0.3f);

                    Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                    Gizmos.DrawSphere(new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z), 0.2f);

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(new Vector3(lastValidPosition.x, feetPosition.y + 0.7f, lastValidPosition.z), "Last Valid Position");
#endif
                }

                if (Vector3.Distance(playerTransform.position, lastValidPosition) > 0.3f)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(feetPosition, new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z));
                }

                if (currentMoveInput.magnitude > 0.1f)
                {
                    Vector3 currentInputDirection = Vector3.zero;
                    if (inputService != null)
                    {
                        currentInputDirection = inputService.CameraRelativeInput;
                    }

                    if (currentInputDirection.magnitude > 0.1f)
                    {
                        Vector3 inputDirection = currentInputDirection.normalized;
                        Vector3 desiredPos = playerTransform.position + inputDirection * gizmoLookAheadTime * Speed;

                        NavMeshHit hit;
                        bool isValidPosition = NavMesh.SamplePosition(desiredPos, out hit, navMeshSampleDistance, NavMesh.AllAreas);

                        float cubeHeight = playerTransform.position.y + 0.1f;

                        if (isValidPosition)
                        {
                            float distance = Vector3.Distance(desiredPos, hit.position);
                            if (distance <= 1f)
                            {
                                Gizmos.color = Color.green;
                                Gizmos.DrawWireCube(new Vector3(hit.position.x, cubeHeight, hit.position.z), Vector3.one * 0.3f);

                                Gizmos.color = Color.green;
                                Gizmos.DrawLine(new Vector3(playerTransform.position.x, cubeHeight, playerTransform.position.z),
                                    new Vector3(hit.position.x, cubeHeight, hit.position.z));
                            }
                            else
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawWireCube(new Vector3(desiredPos.x, cubeHeight, desiredPos.z), Vector3.one * 0.3f);

                                Gizmos.color = Color.red;
                                Gizmos.DrawLine(new Vector3(playerTransform.position.x, cubeHeight, playerTransform.position.z),
                                    new Vector3(desiredPos.x, cubeHeight, desiredPos.z));
                            }
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireCube(new Vector3(desiredPos.x, cubeHeight, desiredPos.z), Vector3.one * 0.3f);

                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(new Vector3(playerTransform.position.x, cubeHeight, playerTransform.position.z),
                                new Vector3(desiredPos.x, cubeHeight, desiredPos.z));
                        }
                    }
                }
            }
        }

        protected virtual Vector3 GetFeetPosition()
        {
            Collider col = playerTransform.GetComponent<Collider>();
            if (col != null)
            {
                return new Vector3(playerTransform.position.x, col.bounds.min.y, playerTransform.position.z);
            }

            Renderer renderer = playerTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                return new Vector3(playerTransform.position.x, renderer.bounds.min.y, playerTransform.position.z);
            }

            return playerTransform.position + Vector3.down * 1f;
        }

        public Vector3 GetCurrentVelocity() => currentMoveInput;
        public Vector3 GetSmoothedMovement() => smoothedMovement;
        public Vector3 GetLastInputDirection() => lastInputDirection;
        public Vector3 GetLastValidPosition() => lastValidPosition;
        public bool IsUsingNavMeshValidation() => useNavMeshValidation;
        public float GetPlayerHeightOffset() => playerHeightOffset;

        public virtual void SnapToNavMeshSurface()
        {
            if (!useNavMeshValidation || playerTransform == null) return;

            NavMeshHit hit;
            Vector3 checkPosition = new Vector3(playerTransform.position.x, playerTransform.position.y - playerHeightOffset, playerTransform.position.z);

            if (NavMesh.SamplePosition(checkPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                Vector3 correctedPosition = new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
                playerTransform.position = correctedPosition;
                lastValidPosition = correctedPosition;
            }
        }
    }
}