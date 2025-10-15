using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Muscular Beta Major", menuName = "Mutations/Effects/Muscular System/Beta Major")]
    public class MuscularBetaMajorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private SlowBulletModifierSO slowModifierSO;

        [Header("Fire Rate Settings")]
        [SerializeField] private float fireRateMultiplier = 1.5f; // 50% más rápido

        private void Awake()
        {
            radiationType = MutationType.Beta;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Muscular Beta Major";
            description = "Disparos rápidos que aplican ralentización.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(slowModifierSO);
                Debug.Log("[MuscularBetaMajorEffect] Activada (slow + fire rate)");

                // aplicar modificación de fire rate en stats del jugador
                var model = player.GetComponent<PlayerModel>();
                if (model != null && model.StatContext != null && model.StatContext.Target != null)
                {
                    model.StatContext.Target.AddFlatBonus(model.StatRefs.fireRate, fireRateMultiplier - 1f);
                }
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(slowModifierSO);
                Debug.Log("[MuscularBetaMajorEffect] Desactivada");
            }

            // revertir modificación de fire rate
            var model = player.GetComponent<PlayerModel>();
            if (model != null && model.StatContext != null && model.StatContext.Target != null)
            {
                model.StatContext.Target.AddFlatBonus(model.StatRefs.fireRate, -(fireRateMultiplier - 1f));
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}

