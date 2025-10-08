using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Neutrons Muscular Minor", menuName = "Mutations/Effects/Muscular System/Neutrons Minor")]
    public class NeutronsMuscularMinorEffect : StatModifierEffect
    {
        [SerializeField] private int enemiesToHit = 8;
        [SerializeField] private float timeToRecover = 3f;

        [Header("Bullet Modifier")]
        [SerializeField] private CounterBulletModifierSO counterModifierSO;
        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Neutrons Minor";
            description = $"Cada 8 disparos acertados sin fallar devuelven +3s de tiempo vital";
            isTemporary = false;
        }


        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(counterModifierSO);
                controller.SetMuscularNeutronsMinor(enemiesToHit, timeToRecover);
                Debug.Log("[MinorEffect] Neutrons Minor activada");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(counterModifierSO);
                controller.UnSetMuscularNeutronsMinor();
                Debug.Log("[MinorEffect] Neutrons Minor desactivada");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
