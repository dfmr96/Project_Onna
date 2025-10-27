using Mutations.Core.Categories;
using Player;
using UnityEngine;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Cherenkov Muscular Major", menuName = "Mutations/Effects/Muscular System/Cherenkov Major")]
    public class CherenkovMuscularMajorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private MarkerBulletModifierSO markerModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Cherenkov;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Cherenkov Major";
            description = "Shots mark enemies they hit, causing them to take more damage from all sources";
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
