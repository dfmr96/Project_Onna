using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    public void SetLevelName(string levelName) { levelText.text = levelName; }
    public void DestroyAfterAnim() { Destroy(gameObject); }
}
