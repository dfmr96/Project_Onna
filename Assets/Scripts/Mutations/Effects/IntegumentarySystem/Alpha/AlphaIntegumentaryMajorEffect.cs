using Mutations.Core;
using Player;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Alpha Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Alpha Major")]
    public class AlphaIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraDamageEffect behavior;

        [Header("Trigger Settings")]
        [SerializeField] private float cooldown = 2f;
        [SerializeField] private float damageMultiplier = 3f;

        private float lastTriggerTime;
        private AuraController auraCtrl;
        private AuraDamageEffect scaledBehavior;
        private PlayerModel playerModel;

        private void OnEnable()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Alpha Integumentary Major";
            description = "Releases a damaging shockwave when the player takes direct damage.";
        }
        
        private void OnDisable()
        {
            if (playerModel != null)
            {
                playerModel.OnTakeDamage -= OnPlayerDamaged;
                playerModel = null;
            }
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[AlphaIntegumentaryMajor] PlayerModel not found!");
                return;
            }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogError("[AlphaIntegumentaryMajor] No AuraController found on player.");
                return;
            }

            // Clone and scale behavior
            scaledBehavior = ScriptableObject.CreateInstance<AuraDamageEffect>();
            scaledBehavior.damagePerSecond = behavior.damagePerSecond * GetValueAtLevel(level) * damageMultiplier;

            // Subscribe to player’s instance event
            playerModel.OnTakeDamage += OnPlayerDamaged;

            Debug.Log("[AlphaIntegumentaryMajor] Subscribed to player.OnTakeDamage.");

#if UNITY_EDITOR
            Debug.Log($"[AlphaIntegumentaryMajor] Player has {playerModel.GetTakeDamageSubscriberCount()} active OnTakeDamage subscribers");
#endif
        }

        public override void RemoveEffect(GameObject player)
        {
            if (playerModel != null)
            {
                playerModel.OnTakeDamage -= OnPlayerDamaged;
                playerModel = null;
            }

            if (auraCtrl)
            {
                auraCtrl.RemoveAura(auraData.auraId);
                auraCtrl = null;
            }

            Debug.Log("[AlphaIntegumentaryMajor] Cleaned references on RemoveEffect.");
        }

        private void OnPlayerDamaged(float damage)
        {
            if (Time.time - lastTriggerTime < cooldown)
                return;

            lastTriggerTime = Time.time;
            TriggerShockwave();
            Debug.Log($"[AlphaIntegumentaryMajor] Player took {damage} damage — event received.");
        }

        private void TriggerShockwave()
        {
            if (auraCtrl == null) return;

            // Crear aura visual
            auraCtrl.AddAura(auraData, scaledBehavior);

            // Aplicar un único tick de daño instantáneo
            scaledBehavior.OnAuraTick(auraCtrl.transform.position, auraData.radius, LayerMask.GetMask("Enemy"));
            Debug.Log("[AlphaIntegumentaryMajor] Shockwave dealt instant damage.");

            // Retirar aura visual luego de breve delay
            auraCtrl.StartCoroutine(RemoveAuraAfterDelay(0.5f));
        }

        private System.Collections.IEnumerator RemoveAuraAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float dmg = behavior.damagePerSecond * GetValueAtLevel(level) * damageMultiplier;
            return $"Releases a powerful shockwave when hit, dealing {dmg:F1} dmg within {auraData.radius}m (CD {cooldown:F1}s).";
        }
    }
}
