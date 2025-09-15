using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;

namespace Mutations.Core
{
    [CreateAssetMenu(fileName = "Categorized Effect Factory", menuName = "Mutations/Categorized Effect Factory")]
    public class CategorizedEffectFactory : ScriptableObject
    {
        [System.Serializable]
        public class EffectTemplate
        {
            public MutationType radiationType;
            public SystemType systemType;
            public SlotType slotType;
            public RadiationEffect effectPrefab;

            [Header("Category Info")]
            public string categoryName;
            public bool isDOT;
            public bool isAura;
            public bool isProjectileModifier;
            public bool isStatModifier;
            public bool isTimeVital;
            public bool isOrbInteraction;
            public bool isMarking;
        }

        [SerializeField] private List<EffectTemplate> effectTemplates;

        private Dictionary<string, RadiationEffect> effectCache = new Dictionary<string, RadiationEffect>();

        public RadiationEffect CreateEffect(MutationType radiation, SystemType system, SlotType slot)
        {
            string key = $"{radiation}_{system}_{slot}";

            if (effectCache.TryGetValue(key, out RadiationEffect cachedEffect))
            {
                return cachedEffect;
            }

            var template = effectTemplates.Find(t =>
                t.radiationType == radiation &&
                t.systemType == system &&
                t.slotType == slot);

            if (template?.effectPrefab != null)
            {
                var effect = Instantiate(template.effectPrefab);
                effectCache[key] = effect;
                return effect;
            }

            Debug.LogWarning($"[CategorizedEffectFactory] No effect found for {radiation} + {system} + {slot}");
            return null;
        }

        public List<RadiationEffect> GetEffectsByCategory(string categoryName)
        {
            var effects = new List<RadiationEffect>();

            foreach (var template in effectTemplates)
            {
                if (template.categoryName == categoryName && template.effectPrefab != null)
                {
                    string key = $"{template.radiationType}_{template.systemType}_{template.slotType}";

                    if (!effectCache.TryGetValue(key, out RadiationEffect effect))
                    {
                        effect = Instantiate(template.effectPrefab);
                        effectCache[key] = effect;
                    }

                    effects.Add(effect);
                }
            }

            return effects;
        }

        public List<string> GetAvailableCategories()
        {
            var categories = new HashSet<string>();

            foreach (var template in effectTemplates)
            {
                if (!string.IsNullOrEmpty(template.categoryName))
                {
                    categories.Add(template.categoryName);
                }
            }

            return new List<string>(categories);
        }

        public string GetEffectCategory(MutationType radiation, SystemType system, SlotType slot)
        {
            var template = effectTemplates.Find(t =>
                t.radiationType == radiation &&
                t.systemType == system &&
                t.slotType == slot);

            return template?.categoryName ?? "Unknown";
        }

        public bool IsEffectOfType<T>(MutationType radiation, SystemType system, SlotType slot) where T : RadiationEffect
        {
            var effect = CreateEffect(radiation, system, slot);
            return effect is T;
        }

        #if UNITY_EDITOR
        [NaughtyAttributes.Button("Auto-Categorize Effects")]
        private void AutoCategorizeEffects()
        {
            foreach (var template in effectTemplates)
            {
                if (template.effectPrefab == null) continue;

                template.isDOT = template.effectPrefab is DOTEffect;
                template.isAura = template.effectPrefab is AuraEffect;
                template.isProjectileModifier = template.effectPrefab is ProjectileModifierEffect;
                template.isStatModifier = template.effectPrefab is StatModifierEffect;
                template.isTimeVital = template.effectPrefab is TimeVitalEffect;
                template.isOrbInteraction = template.effectPrefab is OrbInteractionEffect;
                template.isMarking = template.effectPrefab is MarkingEffect;

                if (template.isDOT) template.categoryName = "DOT";
                else if (template.isAura) template.categoryName = "Aura";
                else if (template.isProjectileModifier) template.categoryName = "ProjectileModifier";
                else if (template.isStatModifier) template.categoryName = "StatModifier";
                else if (template.isTimeVital) template.categoryName = "TimeVital";
                else if (template.isOrbInteraction) template.categoryName = "OrbInteraction";
                else if (template.isMarking) template.categoryName = "Marking";
                else template.categoryName = "Generic";
            }

            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("[CategorizedEffectFactory] Auto-categorization complete");
        }

        [NaughtyAttributes.Button("Validate All Categories")]
        private void ValidateAllCategories()
        {
            var systems = System.Enum.GetValues(typeof(SystemType));
            var radiations = System.Enum.GetValues(typeof(MutationType));
            var slots = System.Enum.GetValues(typeof(SlotType));

            int missingCount = 0;
            var categoryStats = new Dictionary<string, int>();

            foreach (SystemType system in systems)
            {
                foreach (MutationType radiation in radiations)
                {
                    foreach (SlotType slot in slots)
                    {
                        var template = effectTemplates.Find(t =>
                            t.systemType == system &&
                            t.radiationType == radiation &&
                            t.slotType == slot);

                        if (template?.effectPrefab == null)
                        {
                            Debug.LogWarning($"Missing: {radiation} + {system} + {slot}");
                            missingCount++;
                        }
                        else
                        {
                            string category = template.categoryName ?? "Unknown";
                            categoryStats[category] = categoryStats.GetValueOrDefault(category, 0) + 1;
                        }
                    }
                }
            }

            Debug.Log($"Validation complete. Missing effects: {missingCount}");
            foreach (var stat in categoryStats)
            {
                Debug.Log($"Category '{stat.Key}': {stat.Value} effects");
            }
        }
        #endif
    }
}