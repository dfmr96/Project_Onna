using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Strategy Settings")]
    [SerializeField] private MovementStrategyType currentStrategyType = MovementStrategyType.Combat;
    [SerializeField] private CombatMovementStrategy combatStrategy = new CombatMovementStrategy();
    [SerializeField] private HubMovementStrategy hubStrategy = new HubMovementStrategy();

    [Header("References")]
    [SerializeField] private InputVisualizerGizmos inputVisualizer;

    private IMovementStrategy currentStrategy;

    public enum MovementStrategyType
    {
        Combat,
        Hub
    }

    private void Start()
    {
        if (inputVisualizer == null)
            inputVisualizer = FindObjectOfType<InputVisualizerGizmos>();

        SetMovementStrategy(currentStrategyType);
    }

    private void Update()
    {
        currentStrategy?.Update();
    }

    public void SetMovementStrategy(MovementStrategyType strategyType)
    {
        currentStrategyType = strategyType;

        switch (strategyType)
        {
            case MovementStrategyType.Combat:
                currentStrategy = combatStrategy;
                break;
            case MovementStrategyType.Hub:
                currentStrategy = hubStrategy;
                break;
            default:
                currentStrategy = combatStrategy;
                break;
        }

        currentStrategy?.Initialize(transform, inputVisualizer);
    }

    public void SwitchToCombatMode() => SetMovementStrategy(MovementStrategyType.Combat);
    public void SwitchToHubMode() => SetMovementStrategy(MovementStrategyType.Hub);

    public MovementStrategyType GetCurrentStrategyType() => currentStrategyType;
    public IMovementStrategy GetCurrentStrategy() => currentStrategy;

    // Delegate methods for external access to strategy data
    public Vector3 GetCurrentVelocity() => currentStrategy?.GetCurrentVelocity() ?? Vector3.zero;
    public Vector3 GetSmoothedMovement() => currentStrategy?.GetSmoothedMovement() ?? Vector3.zero;
    public Vector3 GetLastInputDirection() => currentStrategy?.GetLastInputDirection() ?? Vector3.zero;
    public Vector3 GetLastValidPosition() => currentStrategy?.GetLastValidPosition() ?? transform.position;
    public bool IsUsingNavMeshValidation() => currentStrategy?.IsUsingNavMeshValidation() ?? false;
    public float GetPlayerHeightOffset() => currentStrategy?.GetPlayerHeightOffset() ?? 1f;

    public void SnapToNavMeshSurface() => currentStrategy?.SnapToNavMeshSurface();

    private void OnDrawGizmosSelected()
    {
        currentStrategy?.OnDrawGizmosSelected();
    }
}