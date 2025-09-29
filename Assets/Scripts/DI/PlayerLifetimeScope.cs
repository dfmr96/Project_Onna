using Player;
using Player.Movement;
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

        // Register InputService as singleton
        builder.Register<CameraRelativeInputProcessor>(Lifetime.Singleton)
            .AsImplementedInterfaces()
            .AsSelf();
        
    }

    private void Start()
    {
        Debug.Log("PlayerLifetimeScope.Start() - Container ready");

        // Force VContainer to instantiate the InputService
        Container.Resolve<IInputService>();
        
    }
}
