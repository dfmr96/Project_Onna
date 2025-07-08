using Mutations;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/New Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField] private string upgradeName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private int cost;
        [SerializeField] private int maxLevel = 4;
        [SerializeField] private UpgradeEffect upgradeEffect;

        public string UpgradeName => upgradeName;
        public string Description => description;
        public Sprite Icon => icon;
        public int Cost => cost;
        public int MaxLevel => maxLevel;
        public UpgradeEffect UpgradeEffect => upgradeEffect;
    }
}
