using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
    [Header("Shadows")]
    [SerializeField] Toggle shadowsToggle; // Off - On

    //[Header("Anti-Aliasing")]
    //[SerializeField] TextMeshProUGUI antiAliasingText;
    //[SerializeField] Button decreaseAntiAliasingButton;
    //[SerializeField] Button increaseAntiAliasingButton;

    //[Header("Quality")]
    //[SerializeField] TextMeshProUGUI qualityText;
    //[SerializeField] Button decreaseQualityButton;
    //[SerializeField] Button increaseQualityButton;

    //private int currentQualityIndex = 1; // Low - Medium - High
    //private int currentAntiAliasing = 0; // Off - 2x - 4x

    void Start() { Initialize(); }

    private void Initialize()
    {
        bool savedShadowsEnabled = PlayerPrefs.GetInt("ShadowsEnabled", 1) == 1;
        int savedOverallQuality = PlayerPrefs.GetInt("OverallQuality", 1);
        int savedAntiAliasing = PlayerPrefs.GetInt("AntiAliasing", 0);

        shadowsToggle.isOn = savedShadowsEnabled;

        //currentAntiAliasing = savedAntiAliasing;
        //UpdateAntiAliasingText();

        //currentQualityIndex = savedOverallQuality;
        //UpdateQualityText();

        ApplyShadows(savedShadowsEnabled);
        ApplyOverallQuality(savedOverallQuality);
        ApplyAntiAliasing(savedAntiAliasing);

        shadowsToggle.onValueChanged.AddListener(ApplyShadows);



        //decreaseQualityButton.onClick.AddListener(DecreaseQuality);
        //increaseQualityButton.onClick.AddListener(IncreaseQuality);
        //decreaseAntiAliasingButton.onClick.AddListener(DecreaseAntiAliasing);
        //increaseAntiAliasingButton.onClick.AddListener(IncreaseAntiAliasing);
    }

    private void ApplyShadows(bool enabled)
    {
        QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
        PlayerPrefs.SetInt("ShadowsEnabled", enabled ? 1 : 0);
    }

    private void ApplyAntiAliasing(int antiAliasing)
    {
        // Configurar Anti Aliasing
        if (antiAliasing == 0) QualitySettings.antiAliasing = 0;
        else if (antiAliasing == 1) QualitySettings.antiAliasing = 2;
        else if (antiAliasing == 2) QualitySettings.antiAliasing = 4;

        PlayerPrefs.SetInt("AntiAliasing", antiAliasing);
        //UpdateAntiAliasingText();
    }

    private void ApplyOverallQuality(int quality)
    {
        //QualitySettings.SetQualityLevel(quality);
        //PlayerPrefs.SetInt("OverallQuality", quality);

        QualitySettings.SetQualityLevel(0);
        PlayerPrefs.SetInt("OverallQuality", 0);

        //UpdateQualityText();
    }

    //private void DecreaseQuality()
    //{
    //    if (currentQualityIndex > 0)
    //    {
    //        currentQualityIndex--;
    //        ApplyOverallQuality(currentQualityIndex);
    //    }
    //}

    //private void IncreaseQuality()
    //{
    //    if (currentQualityIndex < 2)
    //    {
    //        currentQualityIndex++;
    //        ApplyOverallQuality(currentQualityIndex);
    //    }
    //}

    //private void DecreaseAntiAliasing()
    //{
    //    if (currentAntiAliasing > 0)
    //    {
    //        currentAntiAliasing--;
    //        ApplyAntiAliasing(currentAntiAliasing);
    //    }
    //}

    //private void IncreaseAntiAliasing()
    //{
    //    if (currentAntiAliasing < 2)
    //    {
    //        currentAntiAliasing++;
    //        ApplyAntiAliasing(currentAntiAliasing);
    //    }
    //}

    //private void UpdateQualityText()
    //{
    //    switch (currentQualityIndex)
    //    {
    //        case 0: qualityText.text = "Low"; break;
    //        case 1: qualityText.text = "Medium"; break;
    //        case 2: qualityText.text = "High"; break;
    //    }
    //}

    //private void UpdateAntiAliasingText()
    //{
    //    switch (currentAntiAliasing)
    //    {
    //        case 0: antiAliasingText.text = "Off"; break;
    //        case 1: antiAliasingText.text = "2x"; break;
    //        case 2: antiAliasingText.text = "4x"; break;
    //    }
    //}
}