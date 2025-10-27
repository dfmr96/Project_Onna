using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider musicSlider;

    private void Start() => InitializeSliders();

    private void InitializeSliders()
    {
        masterSlider.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, 1f);
        SFXSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
    }

    private void SetAndSaveVolume(AudioMixerGroup mixer, string parameter, string key, float linearVolume)
    {
        AudioManager.Instance.SetMixerVolume(mixer, parameter, linearVolume);

        PlayerPrefs.SetFloat(key, Mathf.Clamp(linearVolume, 0.0001f, 1f));
        PlayerPrefs.Save();
    }

    public void SetMaster()
    {
        SetAndSaveVolume(
            AudioManager.Instance.GeneralMixer,
            AudioManager.MASTER_KEY,
            AudioManager.MASTER_KEY,
            masterSlider.value);
    }

    public void SetSFX()
    {
        SetAndSaveVolume(
            AudioManager.Instance.SFXMixer,
            AudioManager.SFX_KEY,
            AudioManager.SFX_KEY,
            SFXSlider.value);
    }

    public void SetMusic()
    {
        SetAndSaveVolume(
            AudioManager.Instance.MusicMixer,
            AudioManager.MUSIC_KEY,
            AudioManager.MUSIC_KEY,
            musicSlider.value);
    }
}
