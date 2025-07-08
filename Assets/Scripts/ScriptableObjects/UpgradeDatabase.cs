using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Databases/UpgradeDatabase")]
    public class UpgradeDatabase : ScriptableObject
    {
        [SerializeField] private List<UpgradeData> allUpgrades = new List<UpgradeData>();
        [SerializeField] private SerializedDictionary<string, UpgradeData> lookup;

        public List<UpgradeData> AllUpgrades => allUpgrades;

        public void Init()
        {
            if (lookup != null) return;

            lookup = new SerializedDictionary<string, UpgradeData>();
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade == null) continue;

                string id = upgrade.name;
                if (!lookup.ContainsKey(id))
                    lookup.Add(id, upgrade);
                else
                    Debug.LogWarning($"Duplicate upgrade ID found: {id}");
            }
        }

        public UpgradeData GetUpgrade(string id)
        {
            Init();
            return lookup.TryGetValue(id, out var upgrade) ? upgrade : null;
        }

#if UNITY_EDITOR
        [Button("Buscar todos los UpgradeData del proyecto")]
        public void BuscarTodosLosUpgrades()
        {
            string[] guids = AssetDatabase.FindAssets("t:UpgradeData");
            allUpgrades.Clear();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UpgradeData data = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
                if (data != null && !allUpgrades.Contains(data))
                    allUpgrades.Add(data);
            }

            EditorUtility.SetDirty(this);
            Debug.Log($"UpgradeDatabase: Se encontraron {allUpgrades.Count} upgrades.");
        }

        [Button("Poblar diccionario de upgrades")]
        public void PoblarDiccionario()
        {
            lookup = null;
            Init();
            Debug.Log("UpgradeDatabase: Diccionario poblado.");
        }
#endif
    }
}