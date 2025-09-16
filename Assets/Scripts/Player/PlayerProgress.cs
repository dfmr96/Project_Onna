using Player.Stats;
using Player.Stats.Meta;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerProgress
    {
        public MetaProgressionStats stats = new MetaProgressionStats();

        public string ToJson() => JsonUtility.ToJson(this);
        public static PlayerProgress FromJson(string json) => JsonUtility.FromJson<PlayerProgress>(json);

        //TODO Add methods to save and load progress
    }
}