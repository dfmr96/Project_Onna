using UnityEngine;
using System;

public class OptionsMenu : MonoBehaviour
{
    public Action OnClose;
    public void CloseOptionsMenu() 
    {
        OnClose?.Invoke();
        Destroy(gameObject);
    }

    public void PlaySound(AudioClip audioClip) { AudioManager.Instance?.PlaySFX(audioClip); }
}
