using System;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zoneText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        TurnOnText(false);
    }

    private void TurnOnText(bool isActive = true)
    {
        zoneText.gameObject.SetActive(isActive);
        levelText.gameObject.SetActive(isActive);
    }
    public void SetLevelInfo(string zoneName,string levelName)
    {
        zoneText.SetText(zoneName);
        levelText.SetText(levelName);
        TurnOnText();
    }
    public void DestroyAfterAnim() { Destroy(gameObject); }
}
