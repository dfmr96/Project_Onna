using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Microwaves Nervous Major", menuName = "Mutations/Effects/Nervous System/Microwaves Major")]
    public class MicrowavesNervousMajorEffect : StatModifierEffect
    {
        [Header("Microwaves Major Settings")]

        [SerializeField] private float burnDuration = 3f;
        [SerializeField] private float damagePerTick = 2f;
        [SerializeField] private float bonusDamageIfAlreadyBurned = 5f;


        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Nerve;
            slotType = SlotType.Major;
            effectName = "Microwaves Major";
            description = $"Cada Disparos aplican quemadura aguda de +{burnDuration}. Si el enemigo ya esta quemado el disparo hace mas daño inicial de +{bonusDamageIfAlreadyBurned} de daño";
            isTemporary = true;
        }

        
        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // NO aplicamos el efecto directo al enemigo aquí
            // Solo activamos un flag en PlayerControllerEffect
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.SetMicrowavesMajor(true, burnDuration, damagePerTick, bonusDamageIfAlreadyBurned);
                Debug.Log("[MajorEffect] Microwaves Major activada en Player");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.SetMicrowavesMajor(false);
                Debug.Log("[MajorEffect] Microwaves Major desactivada en Player");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}