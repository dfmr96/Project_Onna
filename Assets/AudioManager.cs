using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Start() { audioSource = GetComponent<AudioSource>(); }
    public void PlayOneShot(AudioClip audioClip) { audioSource.PlayOneShot(audioClip); }
}
