using UnityEngine;
using System.Collections.Generic;

namespace Player
{
    public class PlayerAudioView : MonoBehaviour
    {
        [Header("Audio References")]
        [SerializeField] private AudioSource audioSource;

        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> hurtFxList = new List<AudioClip>();
        [SerializeField] private List<AudioClip> healthFxList = new List<AudioClip>();

        public void Initialize()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        public void PlayDamageSound()
        {
            PlayRandomFromList(hurtFxList);
        }

        public void PlayHealthSound()
        {
            PlayRandomFromList(healthFxList);
        }

        public void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
                audioSource.PlayOneShot(clip);
        }

        private void PlayRandomFromList(List<AudioClip> list)
        {
            if (audioSource == null || list == null || list.Count == 0) return;
            var clip = list[Random.Range(0, list.Count)];
            audioSource.PlayOneShot(clip);
        }

        public void SetVolume(float volume)
        {
            if (audioSource != null)
                audioSource.volume = Mathf.Clamp01(volume);
        }

        public void SetMute(bool mute)
        {
            if (audioSource != null)
                audioSource.mute = mute;
        }
    }
}
