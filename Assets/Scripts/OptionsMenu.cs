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
}
