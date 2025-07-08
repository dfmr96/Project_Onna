using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations.Effects.Logic
{
    [CreateAssetMenu(menuName = "Create DamageIncreaseEffect", fileName = "DamageIncreaseEffect", order = 0)]
    public class DamageIncreaseEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.damage, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.damage, value);
                    break;
                case ValueMode.Multiplier:
                    player.AddMultiplierBonus(statRefs.damage, value);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}