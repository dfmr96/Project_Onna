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