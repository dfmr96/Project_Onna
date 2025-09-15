using Mutations.Effects.NervousSystem;
using UnityEngine;

namespace Mutations.Testing
{
    public class CreateGammaNervousMajorAsset : MonoBehaviour
    {
        [NaughtyAttributes.Button("Create Gamma Nervous Major Asset")]
        public void CreateAsset()
        {
#if UNITY_EDITOR
            // Crear instancia del efecto
            var effect = ScriptableObject.CreateInstance<GammaNervousMajorEffect>();

            // Configurar valores por defecto (ya están en Awake)

            // Crear el asset
            string assetPath = "Assets/ScriptableObjects/Mutations/NervousSystem/GammaNervousMajor.asset";
            UnityEditor.AssetDatabase.CreateAsset(effect, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            // Seleccionar el asset creado
            UnityEditor.Selection.activeObject = effect;
            UnityEditor.EditorGUIUtility.PingObject(effect);

            Debug.Log($"[CreateGammaNervousMajorAsset] Asset created at: {assetPath}");
#else
            Debug.LogWarning("[CreateGammaNervousMajorAsset] This only works in the Unity Editor!");
#endif
        }

        [NaughtyAttributes.Button("Test Asset Values")]
        public void TestAssetValues()
        {
#if UNITY_EDITOR
            string assetPath = "Assets/ScriptableObjects/Mutations/NervousSystem/GammaNervousMajor.asset";
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GammaNervousMajorEffect>(assetPath);

            if (asset != null)
            {
                Debug.Log("=== Testing Asset Values ===");
                Debug.Log($"Radiation Type: {asset.RadiationType}");
                Debug.Log($"System Type: {asset.SystemType}");
                Debug.Log($"Slot Type: {asset.SlotType}");
                Debug.Log($"Effect Name: {asset.EffectName}");
                Debug.Log($"Base Value: {asset.BaseValue}");
                Debug.Log($"Max Level: {asset.MaxLevel}");

                for (int i = 1; i <= 4; i++)
                {
                    Debug.Log($"Level {i}: {asset.GetDescriptionAtLevel(i)}");
                }
            }
            else
            {
                Debug.LogError("[CreateGammaNervousMajorAsset] Asset not found! Create it first.");
            }
#endif
        }
    }
}