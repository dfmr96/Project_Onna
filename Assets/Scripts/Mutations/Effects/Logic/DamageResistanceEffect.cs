using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Runtime;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Blindaje Oseo")]
    public class DamageResistanceEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.damageResistance, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.damageResistance, value);
                    break;
                case ValueMode.Multiplier:
                    player.AddMultiplierBonus(statRefs.damageResistance, value);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}