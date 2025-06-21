using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using System;
using UnityEngine;

[Serializable]
public class PlayerItemsHolder
{
    [SerializeField] private SerializedDictionary<UpgradeData, int> upgradesBuyed = new SerializedDictionary<UpgradeData, int>();
    public SerializedDictionary<UpgradeData, int> UpgradesBuyed => upgradesBuyed;

    public void AddUpgrade(UpgradeData data)
    {
        if (CheckUpgrades(data))
        {
            Debug.Log($"{data.UpgradeName} mejorada 1 nivel");
            // Do something
        }
        else upgradesBuyed.Add(data, 1);
    }

    public bool CheckUpgrades(UpgradeData data) { return upgradesBuyed.ContainsKey(data); }
}
