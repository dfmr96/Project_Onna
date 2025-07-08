using System;
using System.Collections.Generic;
using Mutations;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "SU_00_nombre", menuName = "Store/UpgradeItem", order = 1)]
    public class StoreUpgradeData : ScriptableObject
    {
        [SerializeField] private string upgradeName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private ValueMode valueMode = ValueMode.None;
        [SerializeField] private List<UpgradeLevelData> _levels;
        [SerializeField] private UpgradeEffect upgradeEffect;

        public string UpgradeName => upgradeName;
        public string Description => description;
        public Sprite Icon => icon;
        public int MaxLevel => _levels.Count;
        public UpgradeEffect UpgradeEffect => upgradeEffect;

        public ValueMode Mode => valueMode;

        private UpgradeLevelData GetLevelData(int level)
        {
            if (level >= 0 && level < MaxLevel)
                return _levels[level];

            Debug.LogWarning($"Level {level} is out of range for upgrade '{upgradeName}'.");
            return null;
        }

        public int GetCost(int level) => GetLevelData(level)?.cost ?? -1;
        public float GetValue(int level) => GetLevelData(level)?.value ?? 0f;
    }

    [Serializable]
    public class UpgradeLevelData
    {
        public float value;
        public int cost; 
    }
}
