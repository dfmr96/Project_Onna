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

                    GameMode mode = 
                        (sceneName == "HUB" || sceneName == "HUB_Tutorial")
                            ? GameMode.Hub 
                            : GameMode.Run;

                    _selectedMode = mode;
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