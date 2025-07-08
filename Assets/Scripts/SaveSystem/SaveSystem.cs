using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/playerInventory.json";

    
    public static void SaveInventory(PlayerInventory inventory)
    {
        string json = JsonUtility.ToJson(inventory, true); 
        File.WriteAllText(Path, json);
    }
    
    public static PlayerInventory LoadInventory()
    {
        if (!File.Exists(Path))
            return new PlayerInventory();

        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<PlayerInventory>(json);
    }
    public static void SaveCoins(int newCoins)
    {
        PlayerData data = LoadData();
        data.totalCoins = newCoins;
        WriteData(data);
    }

    public static PlayerData LoadData()
    {
        if (!File.Exists(Path))
            return new PlayerData();

        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    private static void WriteData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path, json);
    }
    public static void DebugInventoryJson()
    {
        string path = Application.persistentDataPath + "/playerInventory.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"[Inventory JSON]\n{json}");
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo de inventario.");
        }
    }
}
