using System;
using Core;
using NaughtyAttributes;
using System.Collections;
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
        public event Action<float> OnTakeDamage;
        public static Action<float> OnUpdateTime;
        public event Action<GameMode> OnGameModeChanged;
        public event Action<bool> OnCanShootChanged;
        public event Action<bool> OnCanMeleeChanged;
        
        [SerializeField] PlayerInventory _playerInventory;
        [SerializeField] private bool devMode;
        [SerializeField] private StatReferences statRefs;

        [Header("Debuff: Enemy DoT Effect")]
        [SerializeField] private GameObject poisonEffectPrefab;
        [SerializeField] private Transform poisonAnchor;
        [SerializeField] private Vector3 poisonOffset;
        private Coroutine enemyDoT = null;
        private float poisonTimeRemaining = 0f;
        private ParticleSystem poisonEffectInstance;

        [Header("Floating Damage Text Effect")] 
        [SerializeField] private float heightTextSpawn = 1.5f;
        [SerializeField] private GameObject floatingTextPrefab;

        [Header("Healing Effect")]
        [SerializeField] private GameObject healEffectPrefab;
        [SerializeField] private Transform healAnchor;
        [SerializeField] private Vector3 healOffset;

        public Vector3 Transform => transform.position;

        public StatReferences StatRefs => statRefs;
        public float Speed => StatContext.Source.Get(statRefs.movementSpeed);
        private float DrainRate => StatContext.Source.Get(statRefs.passiveDrainRate);
        public float MaxHealth => StatContext.Source.Get(statRefs.maxVitalTime);
        public int BulletMaxPenetration => (int)StatContext.Source.Get(statRefs.bulletMaxPenetration);

        public float FireRate => StatContext.Source.Get(statRefs.fireRate);
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

        public bool IsInvulnerable => _isInvulnerable;

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

        private bool _isInvulnerable = false;

        private PlayerControllerEffect _playerControllerEffect;

        private void Awake()
        {
            if (_playerControllerEffect == null)
                _playerControllerEffect = GetComponent<PlayerControllerEffect>();
        }

        private void Start()
        {
            _playerView = GetComponent<PlayerView>();

            // Inicializar el modo de juego basado en la escena
            InitializeGameMode();

            // Auto-enviar PlayerSpawnedSignal si no se ha inicializado el StatContext
            if (_statContext == null)
            {
                //EventBus.Publish(new PlayerSpawnedSignal { PlayerGO = gameObject });
            }

        }

        public void InjectStatContext(PlayerStatContext context)
        {
            _statContext = context;
            _currentTime = StatContext.Runtime?.CurrentEnergyTime ?? float.PositiveInfinity;

            //EventBus.Publish(new PlayerInitializedSignal(this));
            EventBus.Publish(new PlayerInitializedSignal(this, _playerControllerEffect));

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
            if (Input.GetKeyDown(KeyCode.F2)) devMode = !DevMode;

            //Agrega Vida
            if (Input.GetKeyDown(KeyCode.F3))
            {
                _currentTime = 9999999f;
            }

            //Vuelve Player al suelo
            if (Input.GetKeyDown(KeyCode.F4))
            {
                //_currentPosition = new Vector3(transform.localScale.x, 1f, transform.localScale.z);

                Vector3 pos = transform.position;
                pos.y = 0f; 
                transform.position = pos;

            }

            if (!DevMode && GameModeSelector.SelectedMode != GameMode.Hub && passiveDrainEnabled)
                ApplyPassiveDrain();
        }

        public void EnablePassiveDrain(bool enable)
        {
            passiveDrainEnabled = enable;
            if (enable)
                Debug.Log("🔋 Passive Drain enabled.");
            else
                Debug.Log("🔋 Passive Drain disabled.");
        }

        private void ApplyPassiveDrain()
        {
            if (_isInvulnerable) return;

            float damagePerFrame = DrainRate * Time.deltaTime;
            ApplyDamage(damagePerFrame, false, false);
        }

        public void TakeDamage(float timeTaken)
        {
            if (_isInvulnerable) return;

            ApplyDamage(timeTaken, true, true);
            _playerView?.PlayDamageEffect();
        }
        
#if UNITY_EDITOR
        public int GetTakeDamageSubscriberCount()
        {
            return OnTakeDamage?.GetInvocationList().Length ?? 0;
        }
#endif

        public void ApplyDebuffDoT(float dotDuration, float dps)
        {
            if (_isInvulnerable) return;

            if (poisonEffectPrefab != null && poisonEffectInstance == null)
            {
                GameObject go = Instantiate(poisonEffectPrefab, poisonAnchor.position + poisonOffset, Quaternion.identity, poisonAnchor);
                poisonEffectInstance = go.GetComponent<ParticleSystem>();
            }
            else if (poisonEffectInstance != null)
            {
                //Asegurarse de que siga al anchor con el offset
                poisonEffectInstance.transform.position = poisonAnchor.position + poisonOffset;
            }

            poisonTimeRemaining = Mathf.Max(poisonTimeRemaining, dotDuration);

            if (enemyDoT == null)
                enemyDoT = StartCoroutine(EnemyDoTCoroutine(dotDuration, dps));
        }

        private IEnumerator EnemyDoTCoroutine(float dotDuration, float dps)
        {
            if (poisonEffectInstance != null)
                poisonEffectInstance.Play();

            while (poisonTimeRemaining > 0f)
            {
                float damagePerFrame = dps * Time.deltaTime;
                ApplyDamage(damagePerFrame, false, false);

                poisonTimeRemaining -= Time.deltaTime;
                yield return null;
            }

            if (poisonEffectInstance != null)
                poisonEffectInstance.Stop();

            enemyDoT = null;
        }

        public void ApplyDamage(float timeTaken, bool applyResistance, bool isDirectDamage = false)
        {
            if (_isInvulnerable) return;

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
                Debug.Log($"[PlayerModel] Taking {effectiveDamage} dmg — firing OnTakeDamage from {gameObject.name}");
                Debug.Log($"[PlayerModel] OnTakeDamage invoked for {effectiveDamage} dmg — Subscribers: {(OnTakeDamage == null ? 0 : OnTakeDamage.GetInvocationList().Length)}");
                OnTakeDamage?.Invoke(effectiveDamage);
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

            // Instanciar partículas de curación
            if (healEffectPrefab != null)
            {
                Vector3 spawnPos = healAnchor != null 
                    ? healAnchor.position + healOffset 
                    : transform.position;

                GameObject effect = Instantiate(healEffectPrefab, spawnPos, Quaternion.identity, healAnchor != null ? healAnchor : transform);
                var ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(effect, ps.main.duration);
                else
                    Destroy(effect, 2f); // fallback
            }
        }
        
        private void ClampEnergy()
        {
            if (StatContext.Runtime != null)
                StatContext.Runtime.SetCurrentEnergyTime(_currentTime, MaxHealth);
        }

        public void Die() => OnPlayerDie?.Invoke();


        public void InjectInventory(PlayerInventory inventory) => _playerInventory = inventory;

        public void SetGameMode(GameMode newMode)
        {
            if (_currentGameMode != newMode)
            {
                _currentGameMode = newMode;
                OnGameModeChanged?.Invoke(newMode);
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

        public void SetInvulnerable(bool isGod)
        {
            if (_isInvulnerable != isGod)
            {
                _isInvulnerable = isGod;
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
