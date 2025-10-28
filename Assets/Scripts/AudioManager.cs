using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public const string MASTER_KEY = "MasterVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";

    [Header("Mixers & Sources")]
    [SerializeField] private AudioMixerGroup generalMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sFXMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource noteAudioSource;

    [Header("Game Mode Logic")]
    [SerializeField] private AudioClip runModeMusic;
    [SerializeField] private AudioClip hubModeMusic;
    [SerializeField] private List<AudioClip> randomRunSFX;

    [Header("Random SFX Config")]
    [SerializeField] private float minSfxWaitTime = 15f;
    [SerializeField] private float maxSfxWaitTime = 60f;

    public AudioMixerGroup MusicMixer => musicMixer;
    public AudioMixerGroup SFXMixer => sFXMixer;
    public AudioMixerGroup GeneralMixer => generalMixer;

    private bool isInRunMode = false;

    private Coroutine randomSfxCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void PlayNote(AudioClip audioClip) => noteAudioSource.PlayOneShot(audioClip);

    public void PlayMusic(AudioClip audioClip)
    {
        if (audioClip != runModeMusic) StopRunAudio();

        if (musicAudioSource.clip == audioClip && musicAudioSource.isPlaying) return;
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    private void StopRunAudio()
    {
        isInRunMode = false;
        if (randomSfxCoroutine != null)
        {
            StopCoroutine(randomSfxCoroutine);
            randomSfxCoroutine = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameModeSelector.SelectedMode == GameMode.Run)
        {
            if (!isInRunMode)
            {
                isInRunMode = true;
                PlayMusic(runModeMusic);

                if (randomSfxCoroutine != null) StopCoroutine(randomSfxCoroutine);
                randomSfxCoroutine = StartCoroutine(PlayRandomSFXLoop());
            }
        }
        else
        {
            if (isInRunMode)
            {
                isInRunMode = false;
                musicAudioSource.Stop();
                PlayMusic(hubModeMusic);

                if (randomSfxCoroutine != null)
                {
                    StopCoroutine(randomSfxCoroutine);
                    randomSfxCoroutine = null;
                }
            }
        }
    }

    private IEnumerator PlayRandomSFXLoop()
    {
        float initialWait = Random.Range(minSfxWaitTime, maxSfxWaitTime);
        yield return new WaitForSeconds(initialWait);

        while (true)
        {
            if (randomRunSFX != null && randomRunSFX.Count > 0)
            {
                int randomIndex = Random.Range(0, randomRunSFX.Count);
                AudioClip clipToPlay = randomRunSFX[randomIndex];
                PlayNote(clipToPlay);
            }

            float waitTime = Random.Range(minSfxWaitTime, maxSfxWaitTime);

            yield return new WaitForSeconds(waitTime);
        }
    }

    [ContextMenu("Try play note")]
    public void TryToPlayNote() => PlayNote(randomRunSFX[0]);
}
