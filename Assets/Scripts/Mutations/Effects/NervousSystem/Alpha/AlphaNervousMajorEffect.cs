using UnityEngine;
using Player;
using Mutations.Core.Categories;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Alpha Nervous Major", menuName = "Mutations/Effects/Nervous System/Alpha Major")]
    public class AlphaNervousMajorEffect : StatModifierEffect
    {
        [Header("Alpha Major Settings")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        private void Awake()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Nerve;
            slotType = SlotType.Major;
            effectName = "Alpha Invulnerabilidad";
            description = $"Cada orbe recogido otorga invulnerabilidad durante {invulnerabilityDuration} segundos";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null)
            {
                Debug.LogWarning("[Alpha Major] PlayerControllerEffect no encontrado en el player.");
                return;
            }

            controllerEffect.SetAlphaMajor(true, invulnerabilityDuration);
            Debug.Log("[Alpha Major] Activada.");
        }

        public override void RemoveEffect(GameObject player)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null) return;

            controllerEffect.SetAlphaMajor(false);
            Debug.Log("[Alpha Major] Desactivada.");
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
