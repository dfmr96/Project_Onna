using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanelPrefab;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject spawner;
    private GameObject optionsPanelInstance = null;

    public void OptionsMenuButton()
    {
        StartCoroutine(OptionsMenuCoroutine());
    }

    private IEnumerator OptionsMenuCoroutine()
    {
        menuPanel.SetActive(false);
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToNextCoroutine();

        if (optionsPanelInstance == null)
        {
            optionsPanelInstance = Instantiate(optionsPanelPrefab, spawner.transform);
            menuPanel.SetActive(false);

            OptionsMenu optionsMenu = optionsPanelInstance.GetComponent<OptionsMenu>();
            if (optionsMenu != null)
            {
                optionsMenu.OnClose += () =>
                {
                    StartCoroutine(ReactivateMenuAfterTransition());
                };
            }
        }
        else
        {
            Destroy(optionsPanelInstance);
            optionsPanelInstance = null;
            menuPanel.SetActive(true);
        }
    }

    private IEnumerator ReactivateMenuAfterTransition()
    {
        if (UI_MainMenu_ParallaxZoom.Instance != null)
            yield return UI_MainMenu_ParallaxZoom.Instance.ZoomToPreviousCoroutine();
        menuPanel.SetActive(true);
    }
}
