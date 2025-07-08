using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] GameObject loadCanvasPrefab;
    [SerializeField] private string hubLevelName;

    public void PlayButton() 
    {
        levelProgression.ResetProgress();
        SceneManagementUtils.AsyncLoadSceneByName(hubLevelName, loadCanvasPrefab, this);
    }

    public void PlaySound(AudioClip audioClip) { AudioManager.Instance?.PlayOneShot(audioClip); }

    public void ExitButton() { Application.Quit(); }
}
