using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Runtime;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Segunda Alma")]
    public class MaxVitalTimeIncreaseEffect : UpgradeEffect
    {
        [SerializeField] private float increasePercent = 0.2f;

        public override void Apply(IStatTarget player)
        {
            player.AddPercentBonus(statRefs.maxVitalTime, increasePercent);
        }
        
#if UNITY_EDITOR
        [Button("🔬 Test Effect")]
        private void TestEffect()
        {
            if (statRefs == null || statRefs.maxVitalTime == null || testBaseStats == null)
            {
                Debug.LogWarning("⚠️ Falta testBaseStats o statRefs configurados.");
                return;
            }

            var testStats = new RuntimeStats(testBaseStats, testMetaStats, statRefs);
            float before = testStats.Get(statRefs.maxVitalTime);

            Apply(testStats);

            float after = testStats.Get(statRefs.maxVitalTime);
            Debug.Log($"🧪 {name}: MaxVitalTime\nAntes: {before:F2} → Después: {after:F2}");
        }
#endif
    }
}