using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance { get; private set; }

    //Para usarlo->
    //DeathManager.Instance.DestroyObject(_gameObject, 1f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyObject(GameObject obj, float delay = 0f)
    {
        Destroy(obj, delay);
    }
}
