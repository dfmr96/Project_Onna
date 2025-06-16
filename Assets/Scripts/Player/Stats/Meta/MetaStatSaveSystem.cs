using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Player.Stats.Meta
{
    public static class MetaStatSaveSystem
    {
        private const string SaveFileName = "meta_stats.json";

        public static void Save(MetaStatBlock meta, StatRegistry registry)
        {
            var data = meta.ToSerializableDict();
            string json = JsonUtility.ToJson(new MetaStatWrapper(data));
            File.WriteAllText(GetSavePath(), json);
        }

        public static void Load(MetaStatBlock meta, StatRegistry registry)
        {
            if (meta == null || registry == null)
            {
                Debug.LogError("❌ MetaStats o Registry es null en MetaStatSaveSystem.Load");
                return;
            }

            string path = GetSavePath();
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<MetaStatWrapper>(json);
            meta.LoadFromSerializableDict(wrapper.Data, registry);
        }
        
        
        [MenuItem("Meta/Clear Stats")]        
        public static void DeleteSaveFile()
        {
            string path = GetSavePath();
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("🗑️ Archivo meta_stats.json eliminado correctamente.");
            }
            else
            {
                Debug.Log("⚠️ No se encontró ningún archivo meta_stats.json para borrar.");
            }
        }

        private static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }

        [System.Serializable]
        private class MetaStatWrapper
        {
            public List<string> keys = new();
            public List<float> values = new();

            public Dictionary<string, float> Data
            {
                get
                {
                    var dict = new Dictionary<string, float>();
                    for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
                        dict[keys[i]] = values[i];
                    return dict;
                }
            }

            public MetaStatWrapper(Dictionary<string, float> source)
            {
                foreach (var pair in source)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }
        }
    }
}