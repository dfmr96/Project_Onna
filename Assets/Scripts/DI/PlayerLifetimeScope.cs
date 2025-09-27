using Player;
using Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerLifetimeScope : LifetimeScope
{
    [Header("Player References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerModel playerModel;

    protected override void Configure(IContainerBuilder builder)
    {
        // Register Camera
        if (mainCamera == null)
            mainCamera = Camera.main;
        builder.RegisterInstance(mainCamera);

        // Register PlayerInputHandler
        if (playerInputHandler == null)
            playerInputHandler = FindObjectOfType<PlayerInputHandler>();
        builder.RegisterInstance(playerInputHandler);

        // Register PlayerModel
        if (playerModel == null)
            playerModel = FindObjectOfType<PlayerModel>();
        builder.RegisterInstance(playerModel);

        // Register InputService as singleton
        builder.Register<CameraRelativeInputProcessor>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();

        // Register existing MonoBehaviours for injection
       // builder.RegisterComponentInHierarchy<PlayerMovement>();
    }

    private void Start()
    {
        Debug.Log("PlayerLifetimeScope.Start() - Container ready");

        // Force VContainer to instantiate the InputService
        Container.Resolve<IInputService>();

        // Debug registrations
        Debug.Log($"PlayerModel registered: {playerModel != null}");
        if (playerModel != null)
        {
            Debug.Log($"PlayerModel name: {playerModel.name}");
        }
    }
}
