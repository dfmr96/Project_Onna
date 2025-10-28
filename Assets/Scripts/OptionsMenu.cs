using UnityEngine;
using System;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public Action OnClose;
    
    [Header("Paneles")]
    public GameObject AudioOptions;
    public GameObject ScreenOptions;
    public GameObject chains;

    [Header("Botones")]
    public GameObject AudioButton;
    public GameObject ScreenButton;
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
        ScreenButton.SetActive(false);
        AudioButton.SetActive(false);
        chains.SetActive(false);
       
        MainBackButton.SetActive(false);
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToNextCoroutine();

        // Activar el panel correspondiente
        panelToOpen.SetActive(true);
        BackButton.SetActive(true);
        
    }

    private IEnumerator BackCoroutine()
    {
        AudioOptions.SetActive(false);
        ScreenOptions.SetActive(false);
        BackButton.SetActive(false);

        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToPreviousCoroutine();

        chains.SetActive(true);
        ScreenButton.SetActive(true);
        AudioButton.SetActive(true);
        MainBackButton.SetActive(true); 
    }

    private IEnumerator MainBackCoroutine()
    {
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToPreviousCoroutine();
        Destroy(gameObject);
    }
}
