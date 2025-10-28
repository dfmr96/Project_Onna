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
        PlayerHelper.DisableInput();
    }

    public void HandleButton()
    {
        switch (GameModeSelector.SelectedMode)
        {
            case GameMode.Hub:
                Application.Quit();
                break;

            default:
                //Se puede hacer que en vez de volver como tal al hub, el jugador muera capaz
                SceneManagementUtils.AsyncLoadSceneByName("HUB", loadCanvasPrefab, this);
                break;
        }
    }

    public void ResumeGame()
    {
        PlayerHelper.EnableInput();
        transform.parent.gameObject.SetActive(false);
    }
}
