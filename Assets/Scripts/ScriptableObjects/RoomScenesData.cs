using AYellowpaper.SerializedCollections;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct RoomInfo
{
    [SerializeField] private string zone;
    [SerializeField] private string level;

    public string Zone => zone;
    public string Level => level;

    public RoomInfo(string zone, string level)
    {
        this.zone = zone;
        this.level = level;
    }
}

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RoomDatabase", menuName = "Rooms/Room Database", order = 0)]
    public class RoomScenesData : ScriptableObject
    {
        [Header("Database Dictionary")]
        [SerializeField] private SerializedDictionary<string, RoomInfo> rooms;
        public SerializedDictionary<string, RoomInfo> Rooms => rooms;
        
        public RoomInfo GetRoom(string id)
        {
            if (rooms.TryGetValue(id, out RoomInfo roomInfo)) return roomInfo;
            else return default;
        }

#if UNITY_EDITOR
        [Header("Add New Room Entry")]
        [Space]
        [SerializeField] private string ID;

        [SerializeField, BoxGroup("Room Info")] private string zone;
        [SerializeField, BoxGroup("Room Info")] private string level;

        [Button("Add Room Entry")]
        public void AddRoomEntry()
        {
            /*if (sceneAsset == null)
            {
                Debug.LogError("RoomDatabase: No se asignó ninguna escena.");
                return;
            }*/

            //string sceneName = sceneAsset.name;

            if (rooms.ContainsKey(ID))
            {
                Debug.LogWarning($"RoomDatabase: La escena '{ID}' ya existe en el diccionario.");
                return;
            }

            RoomInfo newInfo = new RoomInfo(zone, level);

            rooms.Add(ID, newInfo);

            Debug.Log($"RoomDatabase: Agregada la escena '{ID}' al diccionario.");

            EditorUtility.SetDirty(this);

            // Limpia los campos de entrada para evitar duplicar por accidente
            //sceneAsset = null;
            zone = "";
            level = "";
        }
#endif
    }
}