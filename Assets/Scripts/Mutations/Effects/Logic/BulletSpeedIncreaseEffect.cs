using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations.Effects.Logic
{
    public class BulletSpeedIncreaseEffect : UpgradeEffect
    {
        public override void Apply(IStatTarget player, float value, ValueMode mode)
        {
            switch (mode)
            {
                case ValueMode.Flat:
                    player.AddFlatBonus(statRefs.bulletSpeed, value);
                    break;
                case ValueMode.Percent:
                    player.AddPercentBonus(statRefs.bulletSpeed, value);
                    break;
                case ValueMode.Multiplier:
                    player.AddMultiplierBonus(statRefs.bulletSpeed, value);
                    break;
                case ValueMode.None:
                default:
                    Debug.LogWarning($"[UpgradeEffect] ValueMode is None or unrecognized.");
                    break;
            }
        }
    }
}