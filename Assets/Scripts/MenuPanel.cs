using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] GameObject loadCanvasPrefab;
    [SerializeField] private string hubLevelName;
    [SerializeField] private AudioClip mainMenuMusic;

    private void Start() { AudioManager.Instance?.PlayMusic(mainMenuMusic); }

    public void PlayButton() 
    {
        levelProgression.ResetProgress();
        SceneManagementUtils.AsyncLoadSceneByName(hubLevelName, loadCanvasPrefab, this);
    }

    public void PlaySound(AudioClip audioClip) { AudioManager.Instance?.PlaySFX(audioClip); }

    public void ExitButton() { Application.Quit(); }
}
