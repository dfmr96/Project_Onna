using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float TimeRemaining { get; private set; }
    [Header ("Prefabs")]
    [SerializeField] private GameObject playerSpawner;
    [SerializeField] private GameObject enemySpawner;
    [Header("Doors")]
    [SerializeField] private GameObject[] doors;
    private GameObject player;
    private EnemySpawner _enemySpawner;

    [Header("Enemies Spawners")]
    public OrbSpawner orbSpawner;
    public ProjectileSpawner projectileSpawner;




    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        player = playerSpawner.GetComponent<PlayerSpawner>().SpawnPlayer();
        PlayerHelper.SetPlayer(player);
        PlayerHelper.EnableInput();
        _enemySpawner = enemySpawner.GetComponent<EnemySpawner>();
        PlayerModel.OnPlayerDie += DefeatGame;
        _enemySpawner.OnAllWavesCompleted += WinGame;
    }
    private void WinGame() 
    {
        _enemySpawner.OnAllWavesCompleted -= WinGame;
        OpenDoorDebug();
    } 
    private void DefeatGame()
    {
        PlayerModel.OnPlayerDie -= DefeatGame;
        SceneManagementUtils.LoadSceneByName("HUB");
    }
    private void OpenDoorDebug() 
    {
        foreach (GameObject door in doors) { Destroy(door); }
    }
}
