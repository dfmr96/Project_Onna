using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Create MaxAmmoIncreaseEffect", fileName = "MaxAmmoIncreaseEffect", order = 0)]
    public class MaxAmmoIncreaseEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.maxAmmo, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.maxAmmo, value);
                    break;
                case ValueMode.Multiplier:
                    player.AddMultiplierBonus(statRefs.maxAmmo, value);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}