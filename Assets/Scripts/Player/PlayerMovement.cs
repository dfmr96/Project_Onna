using Player;
using UnityEngine;
using VContainer;

public class PlayerMovement : MonoBehaviour
{
    [Header("Strategy Settings")]
    [SerializeField] private MovementStrategyType currentStrategyType = MovementStrategyType.Combat;
    [SerializeField] private CombatMovementStrategy combatStrategy = new CombatMovementStrategy();
    [SerializeField] private HubMovementStrategy hubStrategy = new HubMovementStrategy();


    private IMovementStrategy currentStrategy;

    private PlayerModel playerModel;

    public enum MovementStrategyType
    {
        Combat,
        Hub
    }

    [Inject]
    public void Construct(PlayerModel playerModel)
    {
        this.playerModel = playerModel;
        // Inicializar inmediatamente después de injection
        Initialize();
    }

    private void Initialize()
    {
        if (playerModel != null)
        {
            playerModel.OnGameModeChanged += OnGameModeChanged;
            // Forzar el evento manualmente
            OnGameModeChanged(playerModel.CurrentGameMode);
            //Debug.Log("Subscribed and forced current mode: " + playerModel.CurrentGameMode);
        }
        else
        {
            SetMovementStrategy(currentStrategyType);
            Debug.Log("PlayerModel not found, using default strategy: " + currentStrategyType);
        }
    }

    /*private void Start()
    {
        // Fallback if injection didn't happen (shouldn't occur with proper VContainer setup)
        if (playerModel == null)
        {
            SetMovementStrategy(currentStrategyType);
            Debug.LogWarning("Fallback: PlayerModel not injected, using default strategy: " + currentStrategyType);
        }
    }*/

    private void OnGameModeChanged(GameMode gameMode)
    {
        //Debug.Log("Game Mode: " + gameMode);
        switch (gameMode)
        {
            case GameMode.Hub:
                SetMovementStrategy(MovementStrategyType.Hub);
                break;
            case GameMode.Run:
                SetMovementStrategy(MovementStrategyType.Combat);
                break;
            default:
                SetMovementStrategy(MovementStrategyType.Hub);
                break;
        }
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

        currentStrategy?.Initialize(transform, null);
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

    private void OnDestroy()
    {
        if (playerModel != null)
        {
            playerModel.OnGameModeChanged -= OnGameModeChanged;
        }
    }
}