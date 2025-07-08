using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;
public class PlayerItemsHolder
{
    [SerializeField] private SerializedDictionary<UpgradeData, int> upgradesBuyed = new SerializedDictionary<UpgradeData, int>();
    public SerializedDictionary<UpgradeData, int> UpgradesBuyed => upgradesBuyed;

    public void AddUpgrade(UpgradeData data)
    {
        if (CheckUpgrades(data))
        {
            if (upgradesBuyed[data] < data.MaxLevel)
            {
                upgradesBuyed[data]++;
                Debug.Log($"{data.UpgradeName} mejorada a nivel {upgradesBuyed[data]}");
            }
            else
            {
                Debug.LogWarning($"{data.UpgradeName} ya está en el nivel máximo.");
            }
        }
        else upgradesBuyed.Add(data, 1);
    }

    public bool CanUpgrade(UpgradeData data)
    {
        if (!upgradesBuyed.ContainsKey(data)) return true;
        return upgradesBuyed[data] < data.MaxLevel;
    }

    public bool CheckUpgrades(UpgradeData data) { return upgradesBuyed.ContainsKey(data); }
}
