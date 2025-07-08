using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject loadCanvasPrefab;
    private void OnEnable() { Time.timeScale = 0f; }
    public void ResumeGame() { Time.timeScale = 1f; Destroy(gameObject); }
    public void BackToHub() { Time.timeScale = 1f; SceneManagementUtils.AsyncLoadSceneByName("HUB", loadCanvasPrefab, this); }
    public void ExitButton() { Time.timeScale = 1f; Application.Quit(); }
}
