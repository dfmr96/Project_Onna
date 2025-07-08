using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Meta;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Upgrades/Generic Upgrade Effect")]
    public class UpgradeEffect : ScriptableObject
    {
        [SerializeField] private StatDefinition stat;
        public StatDefinition Stat => stat;

        public void Apply(IStatTarget player, float value, ValueMode mode)
        {
            if (stat == null)
            {
                Debug.LogWarning("[UpgradeEffect] StatDefinition is null.");
                return;
            }

            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(stat, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(stat, value);
                    break;
                case ValueMode.Multiplier:
                    float reductionFactor = 1f - (value / 100f);
                    player.AddMultiplierBonus(stat, reductionFactor);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}