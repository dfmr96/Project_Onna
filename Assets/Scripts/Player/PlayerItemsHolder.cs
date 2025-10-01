using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Mutations;
using Player.Stats.Interfaces;
using ScriptableObjects;
using UnityEngine;

[System.Serializable]
public class PlayerItemsHolder
{
    // Esta lista se serializa como JSON. El diccionario no.
    [SerializeField] private List<UpgradeSaveData> _upgradesBoughtList = new List<UpgradeSaveData>();
    
    private SerializedDictionary<StoreUpgradeData, int> upgradesBoughtDictionary = new SerializedDictionary<StoreUpgradeData, int>();
    public SerializedDictionary<StoreUpgradeData, int> UpgradesBoughtDictionary => upgradesBoughtDictionary;

    public void AddUpgrade(StoreUpgradeData data)
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
        else
        {
            upgradesBoughtDictionary.Add(data, 1);
            Debug.Log($"{data.UpgradeName} mejorada a nivel {upgradesBoughtDictionary[data]}");
        }
    }

    public bool CanUpgrade(StoreUpgradeData data)
    {
        if (!upgradesBoughtDictionary.ContainsKey(data)) return true;
        return upgradesBoughtDictionary[data] < data.MaxLevel;
    }

    public bool CheckUpgrades(StoreUpgradeData data)
    {
        return upgradesBoughtDictionary.ContainsKey(data);
    }
    
    public void ClearUpgrades()
    {
        UpgradesBoughtDictionary.Clear();
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
        var database = Resources.Load<UpgradeDB>("UpgradeDB");

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
    
    public void ApplyAllUpgradesTo(IStatTarget target)
    {
        foreach (var kvp in upgradesBoughtDictionary)
        {
            var upgradeData = kvp.Key;
            int level = kvp.Value;
            //var upgradeStats = upgradeData.UpgradeEffect.name;
            float value = upgradeData.GetValue(level - 1); // niveles empiezan en 1
            ValueMode mode = upgradeData.Mode;

            if (upgradeData.UpgradeEffect != null)
            {
                upgradeData.UpgradeEffect.Apply(target, value, mode);

                Debug.Log($"📦 Aplicando upgrade: <b>{upgradeData.UpgradeName}</b> | Nivel: {level} | Valor: {value} | Modo: {mode}");
            }
            else
            {
                Debug.LogWarning($"⚠️ UpgradeEffect es null para {upgradeData.name}");
            }
        }
        Debug.Log($"✅ Aplicadas {upgradesBoughtDictionary.Count} mejoras al MetaStatBlock.");
    }
}

[System.Serializable]
public class UpgradeSaveData
{
    public string upgradeId; 
    public int level;
}
