﻿using NaughtyAttributes;
using Player.Stats;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Blindaje Oseo")]
    public class DamageResistanceEffect : UpgradeEffect
    {
        [SerializeField] private float resistanceBonus;

        public override void Apply(RuntimeStats player)
        {
            float bonus = resistanceBonus / 100f;
            player.AddRuntimeBonus(statRefs.damageResistance, bonus);
        }
#if UNITY_EDITOR    
        [Button("🔬 Test Effect")]
        private void TestEffect()
        {
            var testStats = new RuntimeStats(testBaseStats, statRefs);
            float before = testStats.Get(statRefs.damageResistance);
            Apply(testStats);
            float after = testStats.Get(statRefs.damageResistance);
            Debug.Log($"🧪 DamageResistance aplicado:\nAntes: {before:F2}\nDespués: {after:F2}");
        }
#endif
    }
}