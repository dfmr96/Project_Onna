using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjects;

public static class GlobalSceneInitializer
{
    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoadListener()
    {
        SceneManager.sceneLoaded += OnFirstSceneLoaded;
    }

    private static void OnFirstSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (initialized) return; // Evita re-ejecutar si cargas otra escena después

        initialized = true;

        string sceneName = scene.name;

        // Determinar GameMode automáticamente
        if (GameModeSelector.SelectedMode == GameMode.None)
        {
            GameMode selected = sceneName == "HUB" ? GameMode.Hub : GameMode.Run;
            GameModeSelector.SelectedMode = selected;
            Debug.Log($"[GlobalSceneInitializer] GameMode seleccionado automáticamente: {selected} (escena: {sceneName})");
        }

        // Inicializar RoomDatabase y buscar la escena actual
        RoomScenesData roomDB = Resources.Load<RoomScenesData>("RoomsDB");
        if (roomDB == null)
        {
            Debug.LogError("[GlobalSceneInitializer] No se encontró RoomsDB en Resources.");
        }
        else
        {
            RoomInfo roomInfo = roomDB.GetRoom(sceneName);
            if (!string.IsNullOrEmpty(roomInfo.Zone) || !string.IsNullOrEmpty(roomInfo.Level))
            {
                Debug.Log($"[GlobalSceneInitializer] Escena '{sceneName}' detectada como Zona: '{roomInfo.Zone}', Nivel: '{roomInfo.Level}'");
            }
            else
            {
                Debug.LogWarning($"[GlobalSceneInitializer] La escena '{sceneName}' no está registrada en el RoomDatabase.");
            }
        }

        // Ya no necesitamos el listener después de la primera escena cargada
        SceneManager.sceneLoaded -= OnFirstSceneLoaded;
    }
}