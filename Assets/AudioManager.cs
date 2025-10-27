using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public const string MASTER_KEY = "MasterVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";

    [SerializeField] private AudioMixerGroup generalMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sFXMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    public AudioMixerGroup MusicMixer => musicMixer;
    public AudioMixerGroup SFXMixer => sFXMixer;
    public AudioMixerGroup GeneralMixer => generalMixer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => Initialize();

    private void Initialize()
    {
        SetMixerVolume(generalMixer, MASTER_KEY, PlayerPrefs.GetFloat(MASTER_KEY, 1));
        SetMixerVolume(musicMixer, MUSIC_KEY, PlayerPrefs.GetFloat(MUSIC_KEY, 1));
        SetMixerVolume(sFXMixer, SFX_KEY, PlayerPrefs.GetFloat(SFX_KEY, 1));

        musicAudioSource.loop = true;
        musicAudioSource.outputAudioMixerGroup = musicMixer;
        sfxAudioSource.outputAudioMixerGroup = sFXMixer;
    }
    public void SetMixerVolume(AudioMixerGroup mixer, string parameter, float linearVolume)
    {
        float clampedValue = Mathf.Clamp(linearVolume, 0.0001f, 1f);
        float dbVolume = Mathf.Log10(clampedValue) * 20;
        mixer.audioMixer.SetFloat(parameter, dbVolume);
    }

    public void PlaySFX(AudioClip audioClip) => sfxAudioSource.PlayOneShot(audioClip);

    public void PlayMusic(AudioClip audioClip)
    {
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    [ContextMenu("Clear PlayerPrefs")]
    public void Clear() => PlayerPrefs.DeleteAll();
}
