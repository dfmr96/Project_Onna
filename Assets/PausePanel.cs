using Player;
using TMPro;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject loadCanvasPrefab;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private string returnToHubText = "";
    [SerializeField] private string quitGameText = "";

    private void OnEnable()
    {
        switch (GameModeSelector.SelectedMode)
        {
            case GameMode.Hub:
                buttonText.text = quitGameText;
                break;

            default:
                buttonText.text = returnToHubText;
                break;
        }
    }
    public void HandleButton()
    {
        switch (GameModeSelector.SelectedMode)
        {
            case GameMode.Hub:
                Application.Quit();
                break;

            default:
                SceneManagementUtils.AsyncLoadSceneByName("HUB", loadCanvasPrefab, this);
                break;
        }
    }
}
