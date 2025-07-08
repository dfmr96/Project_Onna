using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Runtime;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Sangre Maldita")]
    public class GlassCannonEffect : UpgradeEffect
    {
        [SerializeField] private float damageMultiplier = 2f;
        [SerializeField] private float vitalReduction = 0.5f;

        public override void Apply(IStatTarget  player)
        {
            player.AddMultiplierBonus(statRefs.damage, damageMultiplier);
            player.AddMultiplierBonus(statRefs.passiveDrainRate, 1f + vitalReduction);        
        }
        
#if UNITY_EDITOR

        [Button("🔬 Test Effect")]
        private void TestEffect()
        {
            if (statRefs == null || testBaseStats == null)
            {
                Debug.LogWarning("⚠️ Faltan statRefs o testBaseStats para testear.");
                return;
            }

            var testStats = new RuntimeStats(testBaseStats, testMetaStats, statRefs);

            float dmgBefore = testStats.Get(statRefs.damage);
            float hpBefore = testStats.Get(statRefs.maxVitalTime);

            Apply(testStats);

            float dmgAfter = testStats.Get(statRefs.damage);
            float hpAfter = testStats.Get(statRefs.maxVitalTime);

            Debug.Log($"🧪 {name}:\n" +
                      $"- Damage: {dmgBefore:F2} → {dmgAfter:F2}\n" +
                      $"- MaxVitalTime: {hpBefore:F2} → {hpAfter:F2}");
        }
#endif
    }
}