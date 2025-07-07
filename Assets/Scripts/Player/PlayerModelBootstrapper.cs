using System;
using Core;
using Player.Stats;
using Player.Stats.Meta;
using Player.Stats.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerModelBootstrapper : MonoBehaviour
    {
        public static PlayerModelBootstrapper Instance { get; private set; }
        [SerializeField] private GameMode currentMode;

        [Header("Stats Setup")]
        [SerializeField] private StatBlock baseStats;
        [SerializeField] private MetaStatBlock metaStats;
        [SerializeField] private StatReferences statRefs;
        [SerializeField] private StatRegistry registry;
        
        private PlayerStatContext _statContext;

        public MetaStatBlock MetaStats => metaStats;

        public StatRegistry Registry => registry;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (!ValidateDependencies()) return;
            
            MetaStatSaveSystem.Load(metaStats, registry);
            
            EventBus.Publish(new PlayerModelBootstrapperSignal(this));
        }

        private void OnEnable()
        {
            //Debug.Log("🛰 Bootstrapper suscribiéndose al PlayerSpawnedSignal");
            EventBus.Subscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerSpawnedSignal>(OnPlayerSpawned);
        }
        private bool ValidateDependencies()
        {
            bool isValid = true;

            if (metaStats == null)
            {
                Debug.LogError("❌ MetaStats no asignado en el Inspector.");
                isValid = false;
            }

            if (registry == null)
            {
                Debug.LogError("❌ Registry no asignado en el Inspector.");
                isValid = false;
            }

            return isValid;
        }

        private void OnPlayerSpawned(PlayerSpawnedSignal signal)
        {
            //Debug.Log("🧠 Bootstrapper: Recibida señal de jugador spawneado");
            var playerGO = signal.PlayerGO;
            var playerModel = playerGO.GetComponent<PlayerModel>();
            if (playerModel == null)
            {
                Debug.LogError("❌ PlayerModel no encontrado en el jugador instanciado.");
                return;
            }

            _statContext = new PlayerStatContext();

            switch (GameModeSelector.SelectedMode)
            {
                case GameMode.Run:
                    var runtimeStats = RunData.CurrentStats ?? new RuntimeStats(baseStats, metaStats, statRefs);
                    RunData.SetStats(runtimeStats);
                    _statContext.SetupFromExistingRuntime(runtimeStats, metaStats);
                    Debug.Log("<b>🛠 PlayerModelBootstrapper</b>: Inyectando RuntimeStats en PlayerModel.");
                    break;

                case GameMode.Hub:
                    var reader = new MetaStatReader(baseStats, metaStats);
                    _statContext.SetupForHub(reader, metaStats);
                    metaStats.InjectBaseSource(reader);
                    Debug.Log("<b>🛠 PlayerModelBootstrapper</b>: Inyectando MetaStats en PlayerModel.");
                    break;

                default:
                    Debug.LogError("❌ Modo de juego inválido.");
                    return;
            }

            //Debug.Log("✅ StatContext inyectado correctamente en PlayerModel.");
            playerModel.InjectStatContext(_statContext);

        }
    }


    
    
    
}