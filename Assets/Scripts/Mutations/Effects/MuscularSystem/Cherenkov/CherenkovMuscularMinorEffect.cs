using Mutations.Core.Categories;
using Player;
using UnityEngine;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Cherenkov Muscular Minor", menuName = "Mutations/Effects/Muscular System/Cherenkov Minor")]
    public class CherenkovMuscularMinorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private MarkerBulletModifierSO markerModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Cherenkov;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Cherenkov Minor";
            description = "Shots mark enemies they hit, causing them to take a little more damage.";
            isTemporary = false;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.AddBulletModifier(markerModifierSO);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.RemoveBulletModifier(markerModifierSO);
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
