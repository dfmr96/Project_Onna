using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/saveData.save";

    private static readonly string EncryptionKeyBase64 = "Ezy5BxVtShP5Q0iU+YGlBg==";
    private static readonly string EncryptionIVBase64 = "Wb6YkR2N8mLq9FfG4tK1TQ==";

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
            return new SaveData();

        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(SavePath);
            string decryptedJson = Decrypt(encryptedBytes);
            return JsonUtility.FromJson<SaveData>(decryptedJson);
        }
        catch (Exception) { return new SaveData(); }
    }

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            byte[] encryptedBytes = Encrypt(json);
            File.WriteAllBytes(SavePath, encryptedBytes);
        }
        catch (Exception) { }
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

    private static byte[] Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(EncryptionKeyBase64);
            aes.IV = Convert.FromBase64String(EncryptionIVBase64);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs)) { sw.Write(plainText); }

                return ms.ToArray();
            }
        }
    }

    private static string Decrypt(byte[] cipherBytes)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(EncryptionKeyBase64);
            aes.IV = Convert.FromBase64String(EncryptionIVBase64);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs)) { return sr.ReadToEnd(); }
        }
    }
}
