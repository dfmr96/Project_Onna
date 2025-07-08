using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/playerData.json";

    public static void SaveCoins(int newCoins)
    {
        PlayerData data = LoadData();
        data.totalCoins = newCoins;
        WriteData(data);
    }

    public static PlayerData LoadData()
    {
        if (!File.Exists(path))
            return new PlayerData();

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    private static void WriteData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

}
