using UnityEngine;
using System;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public Action OnClose;
    
    [Header("Paneles")]
    public GameObject AudioOptions;
    public GameObject ScreenOptions;
    public GameObject GraphicsOptions;

    [Header("Botones")]
    public GameObject AudioButton;
    public GameObject ScreenButton;
    public GameObject GraphicsButton;
    public GameObject BackButton;
    public GameObject MainBackButton;
    
    public void CloseOptionsMenu()
    {
        OnClose?.Invoke();
        UI_MainMenu_ParallaxZoom.Instance?.PreviousLayer();

        Destroy(gameObject);
    }
    public void OpenAudio()
    {
        StartCoroutine(OpenPanelCoroutine(AudioOptions));
    }

    public void OpenScreen()
    {
        StartCoroutine(OpenPanelCoroutine(ScreenOptions));
    }

    public void OpenGraphics()
    {
        StartCoroutine(OpenPanelCoroutine(GraphicsOptions));
    }

    public void Back()
    {
        StartCoroutine(BackCoroutine());
    }

    public void MainBack()
    {
        StartCoroutine(MainBackCoroutine());
    }

    // --- Coroutines que esperan la transición ---
    
    private IEnumerator OpenPanelCoroutine(GameObject panelToOpen)
    {
        // Llamar al Zoom Next (si aplica)
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToNextCoroutine();

        // Activar el panel correspondiente
        panelToOpen.SetActive(true);
    }

    private IEnumerator BackCoroutine()
    {
        // Llamar al Zoom Previous
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToPreviousCoroutine();

        // Volver a la vista principal de Options
        AudioOptions.SetActive(false);
        ScreenOptions.SetActive(false);
        GraphicsOptions.SetActive(false);
    }

    private IEnumerator MainBackCoroutine()
    {
        // Llamar al Zoom Previous
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToPreviousCoroutine();

        // Destruir este menú y mostrar el menú principal
        Destroy(gameObject);
    }

    public void PlaySound(AudioClip audioClip) { AudioManager.Instance?.PlaySFX(audioClip); }
}
