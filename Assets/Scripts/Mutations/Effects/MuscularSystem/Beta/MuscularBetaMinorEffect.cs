using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Muscular Beta Minor", menuName = "Mutations/Effects/Muscular System/Beta Minor")]
    public class MuscularBetaMinorEffect : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private SlowBulletModifierSO slowModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Beta;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Muscular Beta Minor";
            description = "Disparos aplican ralentización básica.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(slowModifierSO);
                Debug.Log("[MuscularBetaMinorEffect] Activada");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(slowModifierSO);
                Debug.Log("[MuscularBetaMinorEffect] Desactivada");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}

