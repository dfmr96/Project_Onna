using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Runtime;
using UnityEngine;

namespace Mutations.Effects.Logic
{
    [CreateAssetMenu(menuName = "Mutations/Effects/Mirada del Umbral")]
    public class AttackRangeIncreaseEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.attackRange, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.attackRange, value);
                    break;
                case ValueMode.Multiplier:
                    player.AddMultiplierBonus(statRefs.attackRange, value);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}