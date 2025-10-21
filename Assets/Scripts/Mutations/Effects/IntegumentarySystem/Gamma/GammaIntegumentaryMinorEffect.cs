using Mutations.Core;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Gamma Integumentary Minor", menuName = "Mutations/Effects/Integumentary System/Gamma Minor")]
    public class GammaIntegumentaryMinorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraDamageEffect behavior;

        [Header("Pulse Settings")]
        [SerializeField] private float pulseInterval = 4f;  // seconds between pulses
        [SerializeField] private float pulseDuration = 1f;  // how long the aura lasts

        private float pulseTimer;
        private bool isPulsing;
        private GameObject activeVisual;

        private void OnEnable()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Minor;
            effectName = "Gamma Integumentary Minor";
            description = "Emits periodic gamma pulses that damage nearby enemies.";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogWarning("AuraController missing on player.");
                return;
            }

            // Start pulse coroutine via helper
            player.GetComponent<MonoBehaviour>().StartCoroutine(PulseRoutine(auraCtrl, level));
        }

        private System.Collections.IEnumerator PulseRoutine(AuraController auraCtrl, int level)
        {
            float dmg = behavior.damagePerSecond * GetValueAtLevel(level);
            var scaledBehavior = ScriptableObject.CreateInstance<AuraDamageEffect>();
            scaledBehavior.damagePerSecond = dmg;

            while (true)
            {
                // Wait between pulses
                yield return new WaitForSeconds(pulseInterval);

                // Begin pulse
                auraCtrl.AddAura(auraData, scaledBehavior);
                isPulsing = true;
                Debug.Log($"[Gamma Aura Minor] Pulse started ({dmg:F1} dmg/s)");

                // Keep aura for duration
                yield return new WaitForSeconds(pulseDuration);

                // End pulse
                auraCtrl.RemoveAura(auraData.auraId);
                isPulsing = false;
                Debug.Log("[Gamma Aura Minor] Pulse ended");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (auraCtrl)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float dmg = behavior.damagePerSecond * GetValueAtLevel(level);
            return $"Releases a gamma pulse every {pulseInterval:F1}s, dealing {dmg:F1} dmg/s within {auraData.radius}m for {pulseDuration:F1}s.";
        }
    }
}
