using UnityEngine;

namespace Player
{
    public class PlayerAudioView : MonoBehaviour
    {
        [Header("Audio References")]
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip hurtFx;
        [SerializeField] private AudioClip healthFx;

        public void Initialize()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        public void PlayDamageSound()
        {
            if (audioSource != null && hurtFx != null)
            {
                audioSource.PlayOneShot(hurtFx);
            }
        }

        public void PlayHealthSound()
        {
            if (audioSource != null && healthFx != null)
            {
                audioSource.PlayOneShot(healthFx);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public void SetVolume(float volume)
        {
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetMute(bool mute)
        {
            if (audioSource != null)
            {
                audioSource.mute = mute;
            }
        }
    }
}