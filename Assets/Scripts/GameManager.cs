using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float TimeRemaining { get; private set; }
    [Header ("Prefabs")]
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private EnemySpawner enemySpawner;
    [Header("Doors")]
    [SerializeField] private GameObject[] doors;
    private GameObject player;

    [Header("Enemies Spawners")]
    public OrbSpawner orbSpawner;
    public ProjectileSpawner projectileSpawner;

    [SerializeField] private GameObject defeatUIPrefab;
    private GameObject defeatUIInstance;

    //Evento para activar portal tras seleccion de mutacion
    public static event Action OnMutationUIClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        player = playerSpawner.SpawnPlayer();
        PlayerHelper.SetPlayer(player);
        PlayerHelper.EnableInput();
        PlayerModel.OnPlayerDie += DefeatGame;
        enemySpawner.OnAllWavesCompleted += WinGame;
    }
    private void WinGame() 
    {
        enemySpawner.OnAllWavesCompleted -= WinGame;
        OpenDoorDebug();

    }
    private void DefeatGame()
    {
        PlayerModel.OnPlayerDie -= DefeatGame;
        PlayerHelper.DisableInput();
        Time.timeScale = 0f;

        // Instanciar UI de derrota
        if (defeatUIPrefab != null && defeatUIInstance == null)
        {
            defeatUIInstance = Instantiate(defeatUIPrefab);

            // Buscar el botón y asignar callback
            UnityEngine.UI.Button button = defeatUIInstance.GetComponentInChildren<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.AddListener(ReturnToHub);
            }
        }
        else
        {
            Debug.LogError("DefeatUIPrefab no asignado en GameManager.");
        }
    }

    
    private void ReturnToHub()
    {
        GameModeSelector.SelectedMode = GameMode.Hub;
        PlayerHelper.EnableInput();
        Time.timeScale = 1f;
        SceneManagementUtils.LoadSceneByName("HUB");
    }

    private void OpenDoorDebug()
    {
        foreach (GameObject door in doors) { Destroy(door); }
    }
    
    [Button("Link References")]
    public void LinkReferences()
    {
        orbSpawner = FindObjectOfType<OrbSpawner>();
        projectileSpawner = FindObjectOfType<ProjectileSpawner>();
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (orbSpawner.gameObject.activeInHierarchy) Debug.Log("OrbSpawner linked successfully.");
        else Debug.LogError("OrbSpawner not found.");
        if (projectileSpawner.gameObject.activeInHierarchy) Debug.Log("ProjectileSpawner linked successfully.");
        else Debug.LogError("ProjectileSpawner not found.");
        if (playerSpawner.gameObject.activeInHierarchy) Debug.Log("PlayerSpawner linked successfully.");
        else Debug.LogError("PlayerSpawner not found.");
        if (enemySpawner.gameObject.activeInHierarchy) Debug.Log("EnemySpawner linked successfully.");
        else Debug.LogError("EnemySpawner not found.");
        
    }

    public static void RaiseMutationUIClosed()
    {
        OnMutationUIClosed?.Invoke();
    }
}
