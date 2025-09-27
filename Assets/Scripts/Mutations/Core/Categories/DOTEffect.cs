using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public abstract class DOTEffect : RadiationEffect
    {
        [Header("DOT Settings")]
        [SerializeField] protected float dotDuration = 3f;
        [SerializeField] protected float tickRate = 0.5f;
        [SerializeField] protected bool hasSynergy = false;
        [SerializeField] protected float synergyMultiplier = 1.5f;

        public float DOTDuration => dotDuration;
        public float TickRate => tickRate;
        public bool HasSynergy => hasSynergy;
        public float SynergyMultiplier => synergyMultiplier;

        protected float GetDOTDamageAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        protected float GetDOTDurationAtLevel(int level)
        {
            return dotDuration;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            string baseDesc = $"Aplica quemadura de {GetDOTDamageAtLevel(level):F1} de daño durante {GetDOTDurationAtLevel(level):F1}s";

            if (hasSynergy)
                baseDesc += $". Si ya está quemado, aumenta daño inicial x{synergyMultiplier:F1}";

            return baseDesc;
        }

        protected abstract void ApplyDOTToTarget(GameObject target, float damage, float duration, int level);
        protected virtual void OnDOTSynergyTrigger(GameObject target, int level) { }
    }
}