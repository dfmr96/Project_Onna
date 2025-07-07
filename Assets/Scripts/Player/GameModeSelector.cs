using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public enum GameMode
    {
        Hub,
        Run,
        None
    }
    
    public static class GameModeSelector
    {
        private static GameMode? _selectedMode; // Nullable: null significa sin inicializar

        public static GameMode SelectedMode
        {
            get
            {
                if (!_selectedMode.HasValue || _selectedMode.Value == GameMode.None)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    GameMode mode = sceneName == "HUB" ? GameMode.Hub : GameMode.Run;
                    _selectedMode = mode;
                    Debug.Log($"[GameModeSelector] Inicializado automáticamente: {mode} (escena: {sceneName})");
                }
                return _selectedMode.Value;
            }
            set
            {
                _selectedMode = value;
            }
        }
    }
    
    
}