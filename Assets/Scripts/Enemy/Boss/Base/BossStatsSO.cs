using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Boss Logic", menuName = "Enemy Boss Stats/Enemy Boss Stats Base")]
public class BossStatsSO : ScriptableObject
{
    [Header("Vitality")]
    public float MaxHealth = 100f;
    public bool isShieldActive = false;

    [Header("Combat Projectiles")]
    public float ProjectileDamage = 5f;
    public float ShootForce = 15f;
    public int minProjectilesPerBurst = 3;
    public int maxProjectilesPerBurst = 5;
    public float delayBetweenShots = 0.2f;
    public float delayBetweenBursts = 2f;
    public float spreadAngle = 15f;

    [Header("Combat Laser")]
    public float LaserDamagePerSecond = 25f;
    public float LaserTickRate = 0.2f;
    public float LaserLenght = 20f;

    [Header("Attack Range Vision")]
    public float AttackRange = 10f;

    [Header("Vision Combat Stats")]
    public float combatAngle = 30f;
    public LayerMask obstacleCombatLayers;

    [Header("Vision Cone Stats")]
    public float detectionRange = 10f;
    public float visionAngle = 45f;
    public LayerMask obstacleDetectionLayers;

    [Header("Pillar Stats")]
    public float PillarMaxHealth = 100f;


    [Header("Pillar Rastro Orb")]
    public float pillarRadiusSpawnOrb = 1.5f;
    public bool PillarRastroOrbOnHit = true;
    public int pillarNumberOfOrbsOnHit = 1;
    public bool PillarRastroOrbOnDeath = true;
    public int pillarNumberOfOrbsOnDeath = 2;

    [Header("Boss Rastro Orb")]
    public float radiusSpawnOrb = 1.5f;
    public bool RastroOrbOnHit = true;
    public int numberOfOrbsOnHit = 1;
    public bool RastroOrbOnDeath = true;
    public int numberOfOrbsOnDeath = 2;

    [Header("Coins")]
    public int CoinsToDrop = 1;
}

