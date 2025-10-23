using Mutations.Core;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[AlphaIntegumentaryMajor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[AlphaIntegumentaryMajor] OnDisable called - cleaning up all references.");
            CleanupReferences();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                Debug.Log($"[AlphaIntegumentaryMajor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[AlphaIntegumentaryMajor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            if (!ValidateReferences())
            {
                Debug.LogError("[AlphaIntegumentaryMajor] Missing required references. Check auraData and behavior assignments.");
                return;
            }

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

            // Subscribe to player's instance event
            playerModel.OnTakeDamage += OnPlayerDamaged;

            Debug.Log($"[AlphaIntegumentaryMajor] Subscribed to player.OnTakeDamage at level {level}.");

#if UNITY_EDITOR
            Debug.Log($"[AlphaIntegumentaryMajor] Player has {playerModel.GetTakeDamageSubscriberCount()} active OnTakeDamage subscribers");
#endif
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[AlphaIntegumentaryMajor] RemoveEffect called.");
            CleanupReferences();
        }

        private void OnPlayerDamaged(float damage)
        {
            // Validar que las referencias no sean "stale" de una sesión anterior
            if (!IsValidRuntimeState())
            {
                Debug.LogWarning("[AlphaIntegumentaryMajor] OnPlayerDamaged called with stale references - ignoring.");
                return;
            }

            if (Time.time - lastTriggerTime < cooldown)
                return;

            lastTriggerTime = Time.time;
            TriggerShockwave();
            Debug.Log($"[AlphaIntegumentaryMajor] Player took {damage} damage — event received.");
        }

        private void TriggerShockwave()
        {
            if (auraCtrl == null || scaledBehavior == null || auraData == null)
            {
                Debug.LogWarning("[AlphaIntegumentaryMajor] Cannot trigger shockwave - missing references.");
                return;
            }

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

            if (auraCtrl != null && auraData != null)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            float dmg = behavior.damagePerSecond * GetValueAtLevel(level) * damageMultiplier;
            return $"Releases a powerful shockwave when hit, dealing {dmg:F1} dmg within {auraData.radius}m (CD {cooldown:F1}s).";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && behavior != null;
        }

        private bool IsValidRuntimeState()
        {
            // Verificar que todas las referencias runtime sean válidas y no "stale"
            if (playerModel == null || auraCtrl == null || scaledBehavior == null)
                return false;

            // Verificar que el playerModel no esté destruido
            if (playerModel.gameObject == null)
                return false;

            return true;
        }

        private void CleanupReferences()
        {
            Debug.Log("[AlphaIntegumentaryMajor] CleanupReferences - clearing all runtime state.");

            // Desuscribir eventos
            if (playerModel != null)
            {
                try
                {
                    playerModel.OnTakeDamage -= OnPlayerDamaged;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[AlphaIntegumentaryMajor] Exception while unsubscribing: {e.Message}");
                }
                playerModel = null;
            }

            // Limpiar aura
            if (auraCtrl != null)
            {
                try
                {
                    if (auraData != null)
                        auraCtrl.RemoveAura(auraData.auraId);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[AlphaIntegumentaryMajor] Exception while removing aura: {e.Message}");
                }
                auraCtrl = null;
            }

            // Destruir instancia runtime
            if (scaledBehavior != null)
            {
                try
                {
                    Destroy(scaledBehavior);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[AlphaIntegumentaryMajor] Exception while destroying scaledBehavior: {e.Message}");
                }
                scaledBehavior = null;
            }

            // Resetear cooldown
            lastTriggerTime = 0f;
        }
        #endregion
    }
}
