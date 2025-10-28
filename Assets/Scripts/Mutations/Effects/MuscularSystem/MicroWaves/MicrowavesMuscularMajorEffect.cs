using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Microwaves Muscular Major", menuName = "Mutations/Effects/Muscular System/Microwaves Major")]
    public class MicrowavesMuscularMajorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private BurnBulletModifierSO burnModifierSO;


        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Microwaves Major";
            description = "All shots will apply a sharp burn. If the enemy was already burned, the shot then does more initial damage.";
            isTemporary = true;
        }


        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(burnModifierSO); // burnModifierSO es un campo serializado
                Debug.Log("[MajorEffect] Microwaves Major activada");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(burnModifierSO);
                Debug.Log("[MajorEffect] Microwaves Major desactivada");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}