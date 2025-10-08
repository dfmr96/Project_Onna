using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Neutrons Muscular Major", menuName = "Mutations/Effects/Muscular System/Neutrons Major")]
    public class NeutronsMuscularMajorEffect : StatModifierEffect
    {
        [SerializeField] private int enemiesToKill = 9;
        [SerializeField] private float timeToRecover = 10f;
        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Neutrons Major";
            description = $"Cada 9 enemigos eliminados otorgan +10s de tiempo vital (máx. 120s)";
            isTemporary = false;
        }


        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.SetMuscularNeutronsMajor(enemiesToKill, timeToRecover);
                Debug.Log("[MajorEffect] Neutrons Major activada");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.UnSetMuscularNeutronsMajor();
                Debug.Log("[MajorEffect] Neutrons Major desactivada");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
