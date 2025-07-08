using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Create DamageIncreaseEffect", fileName = "DamageIncreaseEffect", order = 0)]
    public class DamageIncreaseEffect : UpgradeEffect
    {
        [SerializeField] private float increaseflat;
        public override void Apply(IStatTarget player)
        {
            player.AddFlatBonus(statRefs.damage, increaseflat);
        }
    }
}