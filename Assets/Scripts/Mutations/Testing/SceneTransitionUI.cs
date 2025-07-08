using Player;
using Player.Stats;
using Player.Stats.Meta;
using Player.Stats.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mutations.Testing
{
    public class SceneTransitionUI : MonoBehaviour
    {
        public void StartRun()
        {
            var bootstrapper = FindObjectOfType<PlayerModelBootstrapper>();
            var metaStats = bootstrapper.MetaStats;
            var registry = bootstrapper.Registry; 
            MetaStatSaveSystem.Save(metaStats, registry);
            GameModeSelector.SelectedMode = GameMode.Run;
            SceneManager.LoadScene("RoomScene");
        }
    
        public void ReturnToHub()
        {
            RunData.Clear();
            GameModeSelector.SelectedMode = GameMode.Hub;
            SceneManager.LoadScene("HubTestScene");
        }
        
        public void GoToNextRoom(string nextRoomSceneName)
        {
            GameModeSelector.SelectedMode = GameMode.Run;
            SceneManager.LoadScene("Room2Scene");
        }
    }
}