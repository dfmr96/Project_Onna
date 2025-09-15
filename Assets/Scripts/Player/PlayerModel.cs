using System;
using Core;
using NaughtyAttributes;
using Player.Stats;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerModel - Componente del patrón MVC que maneja el estado centralizado del jugador
    /// </summary>
    public class PlayerModel : MonoBehaviour, IDamageable, IHealable
    {
        public static Action OnPlayerDie;
        public static Action<float> OnUpdateTime;
        public event Action<GameMode> OnGameModeChanged;
        public event Action<Vector3> OnMovementDirectionChanged;
        public event Action<Vector3> OnRawMovementDirectionChanged;
        public event Action<Vector3> OnAimDirectionChanged;
        public event Action<bool> OnCanShootChanged;
        public event Action<bool> OnCanMeleeChanged;
        
        [SerializeField] PlayerInventory _playerInventory;
        [SerializeField] private bool devMode;
        [SerializeField] private StatReferences statRefs;


        [Header("Floating Damage Text Effect")] 
        [SerializeField] private float heightTextSpawn = 1.5f;
        [SerializeField] private GameObject floatingTextPrefab;


        public StatReferences StatRefs => statRefs;
        public float Speed => StatContext.Source.Get(statRefs.movementSpeed);
        private float DrainRate => StatContext.Source.Get(statRefs.passiveDrainRate);
        public float MaxHealth => StatContext.Source.Get(statRefs.maxVitalTime);
        public float CurrentHealth => _currentTime;
        public float DashCooldown => StatContext.Source.Get(statRefs.dashCooldown);
        public float DashDistance => StatContext.Source.Get(statRefs.dashDistance);
        public PlayerStatContext StatContext => _statContext;
        public bool DevMode => devMode;
        public GameMode CurrentGameMode => _currentGameMode;
        public Vector3 AimDirection => _aimDirection;
        public bool CanShoot => _canShoot;
        public bool CanMelee => _canMelee;
        public PlayerInventory Inventory => _playerInventory;


        private bool passiveDrainEnabled = true;
        private GameMode _currentGameMode;
        private Vector3 _currentPosition;
        private Vector3 _movementDirection;
        private bool _isMoving;
        private Vector3 _aimDirection = Vector3.forward;
        private bool _canShoot = true;
        private bool _canMelee = true;
        private float _currentTime;
        private PlayerStatContext _statContext;
        private PlayerView _playerView;

        private void Start()
        {
            _playerView = GetComponent<PlayerView>();

            // Inicializar el modo de juego basado en la escena
            InitializeGameMode();
        }

        public void InjectStatContext(PlayerStatContext context)
        {
            _statContext = context;
            _currentTime = StatContext.Runtime?.CurrentEnergyTime ?? float.PositiveInfinity;

            EventBus.Publish(new PlayerInitializedSignal(this));
        }

        public void ForceReinitStats()
        {
            /*var oldBonuses = _runtimeStats?.GetAllRuntimeBonuses(); // Necesitarías exponer esto

            _runtimeStats = new RuntimeStats(baseStats, MetaStats, statRefs);
            RunData.SetStats(_runtimeStats);
            CurrentTime = _runtimeStats.CurrentEnergyTime;

            if (oldBonuses != null)
            {
                foreach (var kvp in oldBonuses)
                    _runtimeStats.AddRuntimeBonus(kvp.Key, kvp.Value);
            }*/
        }


        private void Update()
        {
            //if (!_isInitialized) return;
            if (Input.GetKeyDown(KeyCode.F2))
            {
                devMode = !DevMode;
            }

            if (!DevMode && GameModeSelector.SelectedMode != GameMode.Hub && passiveDrainEnabled)
            {
                ApplyPassiveDrain();
            }
        }

        public void EnablePassiveDrain(bool enable)
        {
            passiveDrainEnabled = enable;
            if (enable)
            {
                Debug.Log("🔋 Passive Drain enabled.");
            }
            else
            {
                Debug.Log("🔋 Passive Drain disabled.");
            }
        }

        private void ApplyPassiveDrain()
        {
            float damagePerFrame = DrainRate * Time.deltaTime;
            ApplyDamage(damagePerFrame, false, false);
        }

        public void TakeDamage(float timeTaken)
        {
            ApplyDamage(timeTaken, true, true);

            _playerView?.PlayDamageEffect();
        }

        public void ApplyDamage(float timeTaken, bool applyResistance, bool isDirectDamage = false)
        {
            float resistance = applyResistance ? Mathf.Clamp01(StatContext.Source.Get(statRefs.damageResistance)) : 0f;
            float effectiveDamage = timeTaken * (1f - resistance);

            _currentTime -= effectiveDamage;
            ClampEnergy();

            if (applyResistance)
            {
                // Mostrar texto flotante
                if (floatingTextPrefab != null)
                {
                    Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
                    GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
                    textObj.GetComponent<FloatingDamageText>().Initialize(timeTaken);
                }
            }

            // Shake solo si es daño directo
            if (isDirectDamage)
            {
                var shake = FindObjectOfType<UI_Shake>();
                if (shake != null)
                    shake.Shake(0.25f, 8f);
            }

            OnUpdateTime?.Invoke(_currentTime / StatContext.Source.Get(statRefs.maxVitalTime));

            if (_currentTime <= 0)
                Die();
        }


        public void RecoverTime(float timeRecovered)
        {
            _currentTime = Mathf.Min(_currentTime + timeRecovered, StatContext.Source.Get(statRefs.maxVitalTime));
            ClampEnergy();
            OnUpdateTime?.Invoke(_currentTime / StatContext.Source.Get(statRefs.maxVitalTime));

            _playerView?.PlayHealthEffect();
        }

        private void ClampEnergy()
        {
            if (StatContext.Runtime != null)
                StatContext.Runtime.SetCurrentEnergyTime(_currentTime, MaxHealth);
        }

        public void Die() => OnPlayerDie?.Invoke();


        public void InjectInventory(PlayerInventory inventory)
        {
            _playerInventory = inventory;
        }
        
        public void SetGameMode(GameMode newMode)
        {
            if (_currentGameMode != newMode)
            {
                _currentGameMode = newMode;
                OnGameModeChanged?.Invoke(newMode);
            }
        }

        public void SetPosition(Vector3 newPosition)
        {
            _currentPosition = newPosition;
        }

        public void SetMovementDirection(Vector3 direction)
        {
            if (_movementDirection != direction)
            {
                _movementDirection = direction;
                OnMovementDirectionChanged?.Invoke(direction);

                bool wasMoving = _isMoving;
                _isMoving = direction.sqrMagnitude > 0.01f;
            }
        }

        public void SetRawMovementDirection(Vector3 rawDirection)
        {
            OnRawMovementDirectionChanged?.Invoke(rawDirection);
        }

        public void SetAimDirection(Vector3 direction)
        {
            if (_aimDirection != direction)
            {
                _aimDirection = direction;
                OnAimDirectionChanged?.Invoke(direction);
            }
        }

        public void SetCanShoot(bool canShoot)
        {
            if (_canShoot != canShoot)
            {
                _canShoot = canShoot;
                OnCanShootChanged?.Invoke(canShoot);
            }
        }

        public void SetCanMelee(bool canMelee)
        {
            if (_canMelee != canMelee)
            {
                _canMelee = canMelee;
                OnCanMeleeChanged?.Invoke(canMelee);
            }
        }

        /// <summary>
        /// Inicializa el modo de juego actual para el jugador basado en el modo seleccionado desde GameModeSelector.
        /// </summary>
        /// <remarks>
        /// - Setea el modo de juego del jugador recuperando el modo actualmente seleccionado desde el GameModeSelector. <br/>
        /// - Configura la habildiad de disparo basado en el modo de juego asignado:<br/>
        /// - Dispara es habilitado o deshabilitado dependiendo del modo de juego.<br/>
        /// </remarks>
        public void InitializeGameMode()
        {
            GameMode selectedMode = GameModeSelector.SelectedMode;

            SetGameMode(selectedMode);
            SetCanShoot(_currentGameMode != GameMode.Hub);
            SetCanMelee(_currentGameMode != GameMode.Hub);
            
        }
        
        [Button("Debug OnPlayerDie subscribers")]
        private void DebugOnPlayerDieSubscribers()
        {
            if (OnPlayerDie == null)
            {
                Debug.Log("🛑 OnPlayerDie no tiene suscriptores.");
                return;
            }

            var invocationList = OnPlayerDie.GetInvocationList();
            Debug.Log($"📋 OnPlayerDie tiene {invocationList.Length} suscriptores:");
            foreach (var d in invocationList)
            {
                Debug.Log($"➡️ {d.Method.DeclaringType}.{d.Method.Name} (target: {d.Target})");
            }
        }
    }
}
