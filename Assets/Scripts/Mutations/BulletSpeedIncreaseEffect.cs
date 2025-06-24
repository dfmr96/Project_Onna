using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations
{
    public class BulletSpeedIncreaseEffect : UpgradeEffect
    {
        [SerializeField] private float increasePercent = 0.1f;
        public override void Apply(IStatTarget player)
        {
            player.AddPercentBonus(statRefs.bulletSpeed, increasePercent);
        }
    }
}