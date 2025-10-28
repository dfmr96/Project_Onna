using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOptions : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle vsyncToggle;
    [SerializeField] Slider fpsSlider;
    [SerializeField] TextMeshProUGUI fpsText;

    private Resolution[] resolutions;

    void Start() => Initialize();

    private void Initialize()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> uniqueResolutions = new HashSet<string>();
        List<Resolution> uniqueResolutionObjects = new List<Resolution>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionKey = resolutions[i].width + "x" + resolutions[i].height;
            if (!uniqueResolutions.Contains(resolutionKey))
            {
                uniqueResolutions.Add(resolutionKey);
                uniqueResolutionObjects.Add(resolutions[i]);
                string resolutionText = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(resolutionText);
            }
        }
        resolutions = uniqueResolutionObjects.ToArray();

        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        if (savedResolutionIndex >= resolutions.Length)
        {
            savedResolutionIndex = 0;
        }

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        bool isVsyncEnabled = PlayerPrefs.GetInt("VSync", 1) == 1;
        int savedFps = PlayerPrefs.GetInt("Fps", 60);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.isOn = isFullscreen;
        vsyncToggle.isOn = isVsyncEnabled;

        fpsSlider.minValue = 30;
        fpsSlider.maxValue = 240;
        fpsSlider.wholeNumbers = true;
        fpsSlider.value = savedFps;
        fpsText.text = savedFps + " FPS";

        SetResolution(savedResolutionIndex);
        SetFullscreen(isFullscreen);
        SetVSync(isVsyncEnabled);
        SetFps(savedFps);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        vsyncToggle.onValueChanged.AddListener(SetVSync);
        fpsSlider.onValueChanged.AddListener(SetFps);

        fpsSlider.interactable = !isVsyncEnabled;
    }

    private void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void SetVSync(bool isVsyncEnabled)
    {
        QualitySettings.vSyncCount = isVsyncEnabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVsyncEnabled ? 1 : 0);

        fpsSlider.interactable = !isVsyncEnabled;
    }

    private void SetFps(float fpsValue)
    {
        int fps = Mathf.RoundToInt(fpsValue);

        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("Fps", fps);
        fpsText.text = fps + " FPS";
    }
}
