using Mutations.Core;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Alpha Integumentary Minor", menuName = "Mutations/Effects/Integumentary System/Alpha Minor")]
    public class AlphaIntegumentaryMinorEffect : RadiationEffect
    {
        [Header("Aura (visual)")]
        [SerializeField] private AuraData auraData;

        [Header("Behavior (pushback)")]
        [SerializeField] private AuraPushbackEffect pushBehavior;

        [Header("Trigger")]
        [SerializeField] private float cooldown = 0.4f;
        [SerializeField] private float visualDuration = 0.35f;
        [SerializeField] private LayerMask enemyMask;

        private float lastTriggerTime;
        private PlayerModel playerModel;
        private AuraController auraCtrl;

        private void OnEnable()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Minor;
            effectName = "Alpha Integumentary Minor";
            description = "On taking damage, releases a short shockwave that pushes nearby enemies.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[AlphaMinor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[AlphaMinor] OnDisable called - cleaning up all references.");
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
                Debug.Log($"[AlphaMinor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[AlphaMinor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            if (!ValidateReferences())
            {
                Debug.LogError("[AlphaMinor] Missing required references. Check auraData and pushBehavior assignments.");
                return;
            }

            playerModel = player.GetComponentInChildren<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[AlphaMinor] PlayerModel not found");
                return;
            }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogError("[AlphaMinor] AuraController not found");
                return;
            }

            // Suscribirse al evento
            playerModel.OnTakeDamage += OnPlayerDamaged;

            Debug.Log($"[AlphaMinor] Subscribed to OnTakeDamage at level {level}.");
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[AlphaMinor] RemoveEffect called.");
            CleanupReferences();
        }

        private void OnPlayerDamaged(float dmg)
        {
            // Validar que las referencias no sean "stale" de una sesión anterior
            if (!IsValidRuntimeState())
            {
                Debug.LogWarning("[AlphaMinor] OnPlayerDamaged called with stale references - ignoring.");
                return;
            }

            if (Time.time - lastTriggerTime < cooldown) return;
            lastTriggerTime = Time.time;

            // Visual corto
            auraCtrl.AddAura(auraData, null);

            // Pulso de empuje instantáneo (un solo tick)
            pushBehavior.OnAuraTick(auraCtrl.transform.position, auraData.radius, enemyMask);

            // Quitar el visual luego
            auraCtrl.StartCoroutine(RemoveAuraAfterDelay(visualDuration));

            Debug.Log($"[AlphaMinor] Player took {dmg} damage — pushback triggered.");
        }

        private System.Collections.IEnumerator RemoveAuraAfterDelay(float t)
        {
            yield return new WaitForSeconds(t);

            if (auraCtrl != null && auraData != null)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            return $"On taking damage, emits a short shockwave that pushes enemies within {auraData.radius:F1}m.";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && pushBehavior != null;
        }

        private bool IsValidRuntimeState()
        {
            // Verificar que todas las referencias runtime sean válidas y no "stale"
            if (playerModel == null || auraCtrl == null)
                return false;

            // Verificar que el playerModel no esté destruido
            if (playerModel.gameObject == null)
                return false;

            return true;
        }

        private void CleanupReferences()
        {
            Debug.Log("[AlphaMinor] CleanupReferences - clearing all runtime state.");

            // Desuscribir eventos
            if (playerModel != null)
            {
                try
                {
                    playerModel.OnTakeDamage -= OnPlayerDamaged;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[AlphaMinor] Exception while unsubscribing: {e.Message}");
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
                    Debug.LogWarning($"[AlphaMinor] Exception while removing aura: {e.Message}");
                }
                auraCtrl = null;
            }

            // Resetear cooldown
            lastTriggerTime = 0f;
        }
        #endregion
    }
}
