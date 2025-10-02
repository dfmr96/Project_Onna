using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Microwaves Muscular Minor", menuName = "Mutations/Effects/Muscular System/Microwaves Minor")]
    public class MicrowavesMuscularMinorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private BurnBulletModifierSO burnModifierSO; // Scriptable del modificador


        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Microwaves Minor";
            description = $"Cada Disparos aplican quemadura leve.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(burnModifierSO); // burnModifierSO es un campo serializado
                Debug.Log("[MajorEffect] Microwaves Minor activada");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(burnModifierSO);
                Debug.Log("[MajorEffect] Microwaves Minor desactivada");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}