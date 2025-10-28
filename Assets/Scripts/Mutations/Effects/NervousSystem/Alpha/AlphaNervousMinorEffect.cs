
using UnityEngine;
using Player;
using Enemy.Spawn;
using Mutations.Core.Categories;

namespace Mutations.Effects.NervousSystem
{

    [CreateAssetMenu(fileName = "Alpha Nervous Minor", menuName = "Mutations/Effects/Nervous System/Alpha Minor")]
    public class AlphaNervousMinorEffect : StatModifierEffect
    {
        [Header("Alpha Minor Settings")]
        private float invulnerabilityDuration = 0.2f;

        private void Awake()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Alpha Mini-Invulnerabilidad";
            description = $"Each vital orb collected grants an invulnerability mini-burst of {invulnerabilityDuration} seconds.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null)
            {
                Debug.LogWarning("[Alpha Minor] PlayerControllerEffect no encontrado en el player.");
                return;
            }

            controllerEffect.SetAlphaMinor(true, invulnerabilityDuration);
            Debug.Log("[Alpha Minor] Activada.");
        }

        public override void RemoveEffect(GameObject player)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null) return;

            controllerEffect.SetAlphaMinor(false);
            Debug.Log("[Alpha Minor] Desactivada.");
        }



        protected override void ApplyStatModification(PlayerModel playerModel, int level)
        {
            throw new System.NotImplementedException();
        }

        protected override void RemoveStatModification(PlayerModel playerModel)
        {
            throw new System.NotImplementedException();
        }
    }




}