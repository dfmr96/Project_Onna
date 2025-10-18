using UnityEngine;
using System.Collections;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] GameObject loadCanvasPrefab;
    [SerializeField] private string hubLevelName;
    [SerializeField] private string introLevelName;
    [SerializeField] private AudioClip mainMenuMusic;

    private void Start() => AudioManager.Instance?.PlayMusic(mainMenuMusic);

    public void PlayButton() 
    {
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            StartCoroutine(ZoomToEyeAndStartGame());
        else StartGame();
    }

    private IEnumerator ZoomToEyeAndStartGame()
    {
        var parallax = UI_MainMenu_ParallaxZoom.Instance;
        
        float originalTransition = parallax.transitionTime;
        float originalEyeScale = parallax.eyeScaleFactor;

        // Aceleramos y aumentamos zoom solo para esta transición
        parallax.transitionTime = 0.45f;
        parallax.eyeScaleFactor = 2f; // mucho más grande

        int currentIndex = parallax.CurrentIndex;
        int lastIndex = parallax.LayersCount - 1;

        while (currentIndex < lastIndex)
        {
            yield return parallax.ZoomToNextCoroutine();
            currentIndex = parallax.CurrentIndex;
        }

        // Restauramos valores normales
        parallax.transitionTime = originalTransition;
        parallax.eyeScaleFactor = originalEyeScale;

        StartGame();
    }

    private void StartGame()
    {
        levelProgression.ResetProgress();
        if (SaveSystem.Load().progress.hasSeenIntro) SceneManagementUtils.AsyncLoadSceneByName(hubLevelName, loadCanvasPrefab, this);
        else SceneManagementUtils.AsyncLoadSceneByName(introLevelName, loadCanvasPrefab, this);
    }

    public void PlaySound(AudioClip audioClip) => AudioManager.Instance?.PlaySFX(audioClip);

    public void ExitButton() => Application.Quit();
}
