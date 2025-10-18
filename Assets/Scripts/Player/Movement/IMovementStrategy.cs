using UnityEngine;
using Player;
using Player.Movement;

public interface IMovementStrategy
{
    void Initialize(Transform playerTransform, PlayerModel playerModel);
    void Update();
    void OnDrawGizmosSelected();
    
    Vector3 GetCurrentVelocity();
    Vector3 GetSmoothedMovement();
    Vector3 GetLastInputDirection();
    Vector3 GetLastValidPosition();
    bool IsUsingNavMeshValidation();
    float GetPlayerHeightOffset();
    
    void SnapToNavMeshSurface();
}