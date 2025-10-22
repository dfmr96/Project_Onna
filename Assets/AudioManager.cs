using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sFXMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    public AudioMixerGroup MusicMixer => musicMixer;
    public AudioMixerGroup SFXMixer => sFXMixer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Initialice();
    }

    private void Initialice()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        musicMixer.audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("AudioMusic"));
        sFXMixer.audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("AudioSFX"));

        musicAudioSource.loop = true;
        musicAudioSource.outputAudioMixerGroup = musicMixer;
        sfxAudioSource.outputAudioMixerGroup = sFXMixer;
    }
    public void PlaySFX(AudioClip audioClip) => sfxAudioSource.PlayOneShot(audioClip);
    public void PlayMusic(AudioClip audioClip) 
    {
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }
}
