using UnityEngine;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider musicSlider;

    private void Start() { Initialize(); }

    private void Initialize()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        AudioListener.volume = masterSlider.value;
    }

    public void SetMaster()
    {
        AudioListener.volume = masterSlider.value;
        PlayerPrefs.SetFloat("AudioMaster", masterSlider.value);
        PlayerPrefs.Save();
    }
    public void SetSFX()
    {
        AudioManager.Instance?.MusicMixer.audioMixer.SetFloat("SFXVolume", SFXSlider.value);
        PlayerPrefs.SetFloat("AudioSFX", SFXSlider.value);
        Debug.Log(PlayerPrefs.GetFloat("AudioSFX"));
        PlayerPrefs.Save();
    }
    public void SetMusic()
    {
        AudioManager.Instance?.SFXMixer.audioMixer.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("AudioMusic", musicSlider.value);
        PlayerPrefs.Save();
    }
}
