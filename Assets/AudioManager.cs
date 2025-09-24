using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sFXMixer;
    public AudioMixerGroup MusicMixer => musicMixer;
    public AudioMixerGroup SFXMixer => sFXMixer;
    private AudioSource musicAudioSource;
    private AudioSource sfxAudioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.outputAudioMixerGroup = musicMixer;

        sfxAudioSource = GetComponent<AudioSource>();
        sfxAudioSource.outputAudioMixerGroup = sFXMixer;
    }
    public void PlaySFX(AudioClip audioClip) { sfxAudioSource.PlayOneShot(audioClip); }
    public void PlayMusic(AudioClip audioClip) 
    {
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }
}
