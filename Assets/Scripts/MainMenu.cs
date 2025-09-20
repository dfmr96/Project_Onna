using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanelPrefab;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject spawner;
    private GameObject optionsPanelInstance = null;
    public void OptionsMenu()
    {
        if (optionsPanelInstance == null)
        {
            optionsPanelInstance = Instantiate(optionsPanelPrefab, spawner.transform);
            menuPanel.SetActive(false);

            OptionsMenu optionsMenu = optionsPanelInstance.GetComponent<OptionsMenu>();
            if (optionsMenu != null)
            {
                optionsMenu.OnClose += () => menuPanel.SetActive(true);
            }
        }
        else
        {
            Destroy(optionsPanelInstance);
            optionsPanelInstance = null;
            menuPanel.SetActive(true);
        }
    }
}
