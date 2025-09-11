using UnityEngine;

public enum EnemyVariantType { Yellow, Red, Blue, Green, Purple, Dark }

[CreateAssetMenu(fileName = "EnemyStatsVariant", menuName = "Enemy Stats/Enemy Variant")]
public class EnemyVariantSO : ScriptableObject
{
    public EnemyVariantType variantType;

    [Header("Stats Buffs")]
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    //public float attackChargeTimeMultiplier = 1f;

    [Header("DoT Attacks")]
    public bool hasDoT = false;
    public float dotDamage = 0f;
    public float dotDuration = 0f;

    [Header("Explode On Death")]
    public bool explodesOnDeath = false;
    public float explosionRadius = 0f;
    public float explosionDamage = 0f;

    //public bool hasDefensivePhase = false;
    //public float damageReductionDuringAttack = 0f;

    public int extraOrbsOnDeath = 0;
}
