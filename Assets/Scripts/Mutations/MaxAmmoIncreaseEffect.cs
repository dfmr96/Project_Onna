using Player.Stats.Interfaces;
using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(menuName = "Create MaxAmmoIncreaseEffect", fileName = "MaxAmmoIncreaseEffect", order = 0)]
    public class MaxAmmoIncreaseEffect : UpgradeEffect
    {
        [SerializeField] private float flatIncrease = 2f;
        public override void Apply(IStatTarget player)
        {
            player.AddFlatBonus(statRefs.maxAmmo, flatIncrease);
        }
    }
}