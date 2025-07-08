#if UNITY_EDITOR
using System.IO;
using Mutations;
using Player.Stats;
using UnityEditor;
using UnityEngine;

    public static class UpgradeEffectGenerator
    {
        [MenuItem("Tools/Generate Upgrade Effects")]
        public static void GenerateUpgradeEffects()
        {
            string savePath = "Assets/Game/Upgrades/Effects/"; // Cambialo según tu estructura
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var guids = AssetDatabase.FindAssets("t:StatDefinition");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                StatDefinition stat = AssetDatabase.LoadAssetAtPath<StatDefinition>(path);
                if (stat == null) continue;

                string assetName = $"{stat.name}IncreaseEffect.asset";
                string fullPath = Path.Combine(savePath, assetName);

                // Evitar sobrescribir si ya existe
                if (File.Exists(fullPath))
                {
                    Debug.Log($"🔁 Ya existe: {assetName}, se omite.");
                    continue;
                }

                UpgradeEffect effect = ScriptableObject.CreateInstance<UpgradeEffect>();
            
                SerializedObject so = new SerializedObject(effect);
                so.FindProperty("stat").objectReferenceValue = stat;
                so.ApplyModifiedProperties();

                AssetDatabase.CreateAsset(effect, fullPath);
                Debug.Log($"✅ Creado: {assetName}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif