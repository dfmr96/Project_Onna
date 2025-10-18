using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Logic", menuName = "Enemy Stats/Enemy Stats Base")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Current State")]
    public string currentState = "No State";

    [Header("Vitality")]
    public float MaxHealth = 100f;
    public bool isShieldActive = false;

    [Header("Combat")]
    public float AttackDamage = 20f;
    public float AttackRange = 10f;
    public float AttackTimeRate = 1.5f;
    public float AttackInitialDelay = 0.3f;

    [Header("Vision Combat Stats")]
    public float combatAngle = 30f;
    public LayerMask obstacleCombatLayers;

    [Header("Vision Cone Stats")]
    public float detectionRange = 10f;
    public float visionAngle = 45f;
    public LayerMask obstacleDetectionLayers;

    [Header("Ranged Combat")]
    public float ShootForce = 15f;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 400f;
    public float RandomMovementRange = 30f;

    [Header("Rastro Orb")]
    public float radiusSpawnOrb = 1.5f;
    public bool RastroOrbOnHit = true;
    public int numberOfOrbsOnHit = 1;
    public bool RastroOrbOnDeath = true;
    public int numberOfOrbsOnDeath = 2;

    [Header("Coins")]
    public int CoinsToDrop = 1;
}

