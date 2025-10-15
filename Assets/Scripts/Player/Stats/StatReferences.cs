using UnityEngine;

namespace Player.Stats
{
    [CreateAssetMenu(menuName = "Stats/Stat References", fileName = "StatReferences")]
    public class StatReferences : ScriptableObject
    {
        [Header("Vital Stats")]
        public StatDefinition maxVitalTime;
        public StatDefinition initialVitalTime;
        public StatDefinition passiveDrainRate;
        public StatDefinition enemyHitPenalty;
        public StatDefinition healingMultiplier;

        [Header("Status Flags")]
        public StatDefinition isInvulnerable;

        [Header("Combat Stats")]
        public StatDefinition damage;
        public StatDefinition bulletSpeed;
        public StatDefinition bulletMaxPenetration;
        public StatDefinition maxAmmo;
        public StatDefinition attackRange;
        public StatDefinition fireRate;
        public StatDefinition criticalChance;
        public StatDefinition criticalDamageMultiplier;
        public StatDefinition damageResistance;
        public StatDefinition overheatCooldown;
        public StatDefinition coolingCooldown;

        [Header("Movement Stats")]
        public StatDefinition movementSpeed;
        public StatDefinition dashDistance;
        public StatDefinition dashCooldown;

        [Header("Orb Interaction Stats")]
        public StatDefinition orbAttractRange;
        public StatDefinition orbAttractSpeed;

#if UNITY_EDITOR
        [ContextMenu("Auto Link From Registry")]
        public void AutoLinkFromRegistry()
        {
            var registry = UnityEditor.AssetDatabase.LoadAssetAtPath<StatRegistry>("Assets/Stats/StatRegistry.asset");

            if (!registry)
            {
                Debug.LogWarning("⚠️ No se encontró el StatRegistry en Assets/Stats/StatRegistry.asset");
                return;
            }

            TryAssign(ref maxVitalTime, "MaxVitalTime", registry);
            TryAssign(ref initialVitalTime, "InitialVitalTime", registry);
            TryAssign(ref passiveDrainRate, "PassiveDrainRate", registry);
            TryAssign(ref enemyHitPenalty, "EnemyHitPenalty", registry);
            TryAssign(ref healingMultiplier, "HealingMultiplier", registry);
            TryAssign(ref isInvulnerable, "IsInvulnerable", registry);
            TryAssign(ref damage, "Damage", registry);
            TryAssign(ref attackRange, "AttackRange", registry);
            TryAssign(ref fireRate, "FireRate", registry);
            TryAssign(ref criticalChance, "CriticalChance", registry);
            TryAssign(ref criticalDamageMultiplier, "CriticalDamageMultiplier", registry);
            TryAssign(ref damageResistance, "DamageResistance", registry);
            TryAssign(ref overheatCooldown, "OverheatCooldown", registry);
            TryAssign(ref movementSpeed, "MovementSpeed", registry);
            TryAssign(ref dashDistance, "DashDistance", registry);
            TryAssign(ref dashCooldown, "DashCooldown", registry);
            TryAssign(ref coolingCooldown, "CoolingCooldown", registry);
            TryAssign(ref bulletSpeed, "BulletSpeed", registry);
            TryAssign(ref maxAmmo, "MaxAmmo", registry);
            TryAssign(ref orbAttractRange, "OrbAttractRange", registry);
            TryAssign(ref orbAttractSpeed, "OrbAttractSpeed", registry);
            TryAssign(ref bulletMaxPenetration, "bulletMaxPenetration", registry);

            

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("✅ StatReferences actualizadas desde el StatRegistry.");
        }

        private void TryAssign(ref StatDefinition field, string statName, StatRegistry registry)
        {
            var stat = registry.GetByName(statName);
            if (stat != null)
            {
                field = stat;
            }
            else
            {
                Debug.LogWarning($"⚠️ Stat '{statName}' no encontrada en el StatRegistry.");
            }
        }
#endif
    }
}
