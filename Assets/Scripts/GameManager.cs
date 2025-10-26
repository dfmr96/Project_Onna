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
    [SerializeField] private GameObject pausePrefab;
    [SerializeField] private GameObject loadScreenPrefab;
    [Header("Doors")]
    [SerializeField] private GameObject[] doors;
    private GameObject player;
    [SerializeField] private GameObject deathScreenTransitionPrefab;
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject playerHUD;

    [Header("Spawners")]
    public OrbSpawner orbSpawner;
    public ProjectileSpawner projectileSpawner;

    [SerializeField] private GameObject defeatUIPrefab;
    private GameObject defeatUIInstance;

    //Evento para activar portal tras seleccion de mutacion
    public event Action OnMutationUIClosed;

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
    private void WinGame() => enemySpawner.OnAllWavesCompleted -= WinGame;

    private void DefeatGame()
    {
        PlayerModel.OnPlayerDie -= DefeatGame;
        PlayerHelper.DisableInput();
        Cursor.visible = true;
        playerHUD?.SetActive(false);
        Time.timeScale = 0f;

        StartCoroutine(HandleDefeatSequence());
    }

    private IEnumerator HandleDefeatSequence()
    {
        // Partículas primero
        if (deathParticlesPrefab != null && player != null)
        {
            Instantiate(
                deathParticlesPrefab,
                player.transform.position - new Vector3(0,1.5f,0),
                Quaternion.identity
            );
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (deathScreenTransitionPrefab != null && player != null)
        {
            GameObject transition = Instantiate(
                deathScreenTransitionPrefab,
                player.transform.position,
                Quaternion.identity
            );

            var transitionScript = transition.GetComponent<DeathScreenTransition>();
            if (transitionScript != null)
                transitionScript.SetDefeatUI(defeatUIPrefab);
        }
    }

    public void ReturnToHub()
    {
        GameModeSelector.SelectedMode = GameMode.Hub;
        PlayerHelper.EnableInput();
        Time.timeScale = 1f;
        SceneManagementUtils.AsyncLoadSceneByName("HUB", loadScreenPrefab, this);
        //  NO -- Encima esta hardcodeado
        //SceneManagementUtils.LoadSceneByName("HUB");
    }
    
    public void ReturnToTutorial()
    {
        PlayerHelper.EnableInput();
        Time.timeScale = 1f;
        //  NO -- Encima esta hardcodeado
        SceneManagementUtils.AsyncLoadSceneByName("Z1_L5_Tutorial", loadScreenPrefab, this);
        //SceneManagementUtils.LoadSceneByName("Z1_L5_Tutorial");
    }

    public void ReturnToHubTutorial()
    {
        GameModeSelector.SelectedMode = GameMode.Hub;
        PlayerHelper.EnableInput();
        Time.timeScale = 1f;
        SceneManagementUtils.AsyncLoadSceneByName("HUB_Tutorial", loadScreenPrefab, this);
        //  NO -- Encima esta hardcodeado
        //SceneManagementUtils.LoadSceneByName("HUB_Tutorial");
    }


    public void OpenDoorDebug()
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

    public void OnOrbMutationActivated(OrbMutation orb)
    {
        if (orb != null)
            Destroy(orb.gameObject);

        StartCoroutine(InvokeMutationClosedNextFrame());
    }

    private IEnumerator InvokeMutationClosedNextFrame()
    {
        yield return null;
        OnMutationUIClosed?.Invoke();
        OpenDoorDebug();
    }


}
