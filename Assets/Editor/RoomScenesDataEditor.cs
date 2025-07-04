/*
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ScriptableObjects;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(RoomScenesData))]
public class RoomScenesDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("List All Scenes in Project"))
        {
            ListAllScenes();
        }

        if (GUILayout.Button("Add All Scenes to Dictionary (if missing)"))
        {
            AddAllScenesToDictionary();
        }
    }

    private void ListAllScenes()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        Debug.Log($"Encontradas {guids.Length} escenas en el proyecto:");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (sceneAsset != null)
            {
                Debug.Log($" - {sceneAsset.name} (Path: {path})");
            }
        }
    }

    private void AddAllScenesToDictionary()
    {
        RoomScenesData database = (RoomScenesData)target;

        string[] guids = AssetDatabase.FindAssets("t:Scene");
        int addedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (sceneAsset != null)
            {
                string sceneName = sceneAsset.name;

                // Si el diccionario no tiene la key, la agrega con RoomInfo vacío
                if (!database.Rooms.ContainsKey(sceneName))
                {
                    database.Rooms.Add(sceneName, new RoomInfo());
                    addedCount++;
                }
            }
        }

        if (addedCount > 0)
        {
            EditorUtility.SetDirty(database);
            Debug.Log($"RoomDatabase: Agregadas {addedCount} escenas nuevas al diccionario.");
        }
        else
        {
            Debug.Log("RoomDatabase: No se agregaron escenas nuevas, todas ya estaban en el diccionario.");
        }
    }
}
#endif
*/
