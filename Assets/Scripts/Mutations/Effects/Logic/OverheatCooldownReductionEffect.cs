using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Runtime;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Tactica de Guerra")]
    public class OverheatCooldownReductionEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.overheatCooldown, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.overheatCooldown, value);
                    break;
                case ValueMode.Multiplier:
                    float reductionFactor = 1f - (value / 100f);
                    player.AddMultiplierBonus(statRefs.overheatCooldown, reductionFactor);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}