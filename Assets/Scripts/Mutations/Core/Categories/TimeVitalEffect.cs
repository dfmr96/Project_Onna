using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public enum TimeVitalTrigger
    {
        OnOrbCollected,
        OnEnemyKilled,
        OnDamageReceived,
        OnShotHit,
        OnMeleeHit,
        OnGrenadeHit
    }

    public abstract class TimeVitalEffect : RadiationEffect
    {
        [Header("Time Vital Settings")]
        [SerializeField] protected TimeVitalTrigger triggerType;
        [SerializeField] protected float timeBonus = 2f;
        [SerializeField] protected int maxTimeVital = 120;
        [SerializeField] protected bool canSlowConsumption = false;
        [SerializeField] protected float slowConsumptionDuration = 3f;
        [SerializeField] protected float slowConsumptionRate = 0.5f;

        public TimeVitalTrigger TriggerType => triggerType;
        public float TimeBonus => timeBonus;
        public int MaxTimeVital => maxTimeVital;
        public bool CanSlowConsumption => canSlowConsumption;
        public float SlowConsumptionDuration => slowConsumptionDuration;
        public float SlowConsumptionRate => slowConsumptionRate;

        protected float GetTimeBonusAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            string triggerText = triggerType switch
            {
                TimeVitalTrigger.OnOrbCollected => "orbe recogido",
                TimeVitalTrigger.OnEnemyKilled => "enemigo eliminado",
                TimeVitalTrigger.OnDamageReceived => "daño recibido",
                TimeVitalTrigger.OnShotHit => "disparo acertado",
                TimeVitalTrigger.OnMeleeHit => "golpe melee",
                TimeVitalTrigger.OnGrenadeHit => "impacto de granada",
                _ => "acción"
            };

            string baseDesc = $"Cada {triggerText} otorga +{GetTimeBonusAtLevel(level):F1}s de tiempo vital";

            if (canSlowConsumption)
                baseDesc += $". Ralentiza consumo durante {slowConsumptionDuration:F1}s";

            return baseDesc + $" (máx. {maxTimeVital}s)";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // TODO: Implementar integración con sistema de tiempo vital del juego
            Debug.Log($"[TimeVitalEffect] Applied {EffectName} at level {level}");

            RegisterTimeVitalEffect(player, level);
        }

        public override void RemoveEffect(GameObject player)
        {
            // TODO: Implementar remoción de efecto de tiempo vital
            Debug.Log($"[TimeVitalEffect] Removed {EffectName}");

            UnregisterTimeVitalEffect(player);
        }

        protected virtual void RegisterTimeVitalEffect(GameObject player, int level) { }
        protected virtual void UnregisterTimeVitalEffect(GameObject player) { }
    }
}