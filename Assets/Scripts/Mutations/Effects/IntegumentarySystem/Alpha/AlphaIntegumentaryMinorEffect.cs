using Mutations.Core;
using Player;
using UnityEngine;

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
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            playerModel = player.GetComponentInChildren<PlayerModel>();
            if (!playerModel) { Debug.LogError("[AlphaMinor] PlayerModel not found"); return; }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl) { Debug.LogError("[AlphaMinor] AuraController not found"); return; }

            // Por seguridad: evitar dobles suscripciones si se re-aplica
            playerModel.OnTakeDamage -= OnPlayerDamaged;
            playerModel.OnTakeDamage += OnPlayerDamaged;

            Debug.Log("[AlphaMinor] Subscribed to OnTakeDamage.");
        }

        public override void RemoveEffect(GameObject player)
        {
            if (playerModel != null)
            {
                playerModel.OnTakeDamage -= OnPlayerDamaged;
                playerModel = null;
            }
            if (auraCtrl) auraCtrl.RemoveAura(auraData.auraId);
        }

        private void OnDisable()
        {
            // Limpieza por si el SO persiste entre play sessions
            if (playerModel != null)
            {
                playerModel.OnTakeDamage -= OnPlayerDamaged;
                playerModel = null;
            }
        }

        private void OnPlayerDamaged(float dmg)
        {
            if (Time.time - lastTriggerTime < cooldown) return;
            lastTriggerTime = Time.time;

            // Visual corto
            auraCtrl.AddAura(auraData, null);

            // Pulso de empuje instantáneo (un solo tick)
            pushBehavior.OnAuraTick(auraCtrl.transform.position, auraData.radius, enemyMask);

            // Quitar el visual luego
            auraCtrl.StartCoroutine(RemoveAuraAfterDelay(visualDuration));
        }

        private System.Collections.IEnumerator RemoveAuraAfterDelay(float t)
        {
            yield return new WaitForSeconds(t);
            if (auraCtrl) auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            return $"On taking damage, emits a short shockwave that pushes enemies within {auraData.radius:F1}m.";
        }
    }
}
