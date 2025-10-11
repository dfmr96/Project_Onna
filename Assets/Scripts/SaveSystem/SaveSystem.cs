using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/saveData.json";

    public static SaveData Load()
    {
        if (!File.Exists(Path))
            return new SaveData();

        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
    }

    public static void SaveInventory(PlayerInventory newInventory)
    {
        var data = Load();
        data.inventory = newInventory;
        Save(data);
    }

    public static void SaveCoins(int newCoins)
    {
        var data = Load();
        data.playerData.totalCoins = newCoins;
        Save(data);
    }

    public static void MarkIntroSeen()
    {
        var data = Load();
        data.progress.hasSeenIntro = true;
        Save(data);
    }
}
