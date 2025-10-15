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
        [SerializeField] private BurnBulletModifierSO burnModifierSO; // Scriptable del modificador


        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Microwaves Major";
            description = $"Cada Disparos aplican quemadura aguda. Si el enemigo ya esta quemado el disparo hace mas daño inicial";
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