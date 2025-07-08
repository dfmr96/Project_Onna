using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;

[System.Serializable]
public class PlayerItemsHolder
{
    [SerializeField] private SerializedDictionary<UpgradeData, int> upgradesBoughtDictionary = new SerializedDictionary<UpgradeData, int>();
    
    // Esta lista se serializa como JSON. El diccionario no.
    [SerializeField] private List<UpgradeSaveData> _upgradesBoughtList = new List<UpgradeSaveData>();
    public SerializedDictionary<UpgradeData, int> UpgradesBoughtDictionary => upgradesBoughtDictionary;

    public void AddUpgrade(UpgradeData data)
    {
        if (CheckUpgrades(data))
        {
            if (upgradesBoughtDictionary[data] < data.MaxLevel)
            {
                upgradesBoughtDictionary[data]++;
                Debug.Log($"{data.UpgradeName} mejorada a nivel {upgradesBoughtDictionary[data]}");
            }
            else
            {
                Debug.LogWarning($"{data.UpgradeName} ya est� en el nivel m�ximo.");
            }
        }
        else upgradesBoughtDictionary.Add(data, 1);
    }

    public bool CanUpgrade(UpgradeData data)
    {
        if (!upgradesBoughtDictionary.ContainsKey(data)) return true;
        return upgradesBoughtDictionary[data] < data.MaxLevel;
    }

    public bool CheckUpgrades(UpgradeData data)
    {
        return upgradesBoughtDictionary.ContainsKey(data);
    }
    
    /// <summary>
    /// Prepara la lista `upgrades` para que se pueda guardar como JSON.
    /// Llamar antes de serializar.
    /// </summary>
    public void PrepareForSave()
    {
        _upgradesBoughtList.Clear();
        foreach (var pair in upgradesBoughtDictionary)
        {
            _upgradesBoughtList.Add(new UpgradeSaveData
            {
                upgradeId = pair.Key.name,
                level = pair.Value
            });
        }
    }
    
    /// <summary>
    /// Reconstruye el diccionario `upgradesBuyed` a partir de la lista `upgrades`.
    /// Llamar después de cargar del JSON.
    /// </summary>
    public void RestoreFromSave()
    {
        upgradesBoughtDictionary.Clear();
        var database = Resources.Load<UpgradeDatabase>("UpgradeDB");

        if (database == null)
        {
            Debug.LogError("UpgradeDatabase no encontrado en Resources.");
            return;
        }
        foreach (var saved in _upgradesBoughtList)
        {
            var upgrade = database.GetUpgrade(saved.upgradeId);
            if (upgrade != null)
            {
                upgradesBoughtDictionary.Add(upgrade, saved.level);
            }
            else
            {
                Debug.LogWarning($"Upgrade '{saved.upgradeId}' no encontrado en UpgradeDatabase.");
            }
        }
    }
}

[System.Serializable]
public class UpgradeSaveData
{
    public string upgradeId; 
    public int level;
}
