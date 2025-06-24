using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class HubBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerSpawner spawner;
    [SerializeField] private HubManager hubManager;

    private void Awake()
    {
        InitPlayer();
        hubManager.Init();
    }
    
    
    private void InitPlayer()
    {
        spawner.SpawnPlayer();
        PlayerHelper.EnableInput();
    }
}
