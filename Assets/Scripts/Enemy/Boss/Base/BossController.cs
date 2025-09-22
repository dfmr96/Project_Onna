using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Player;
using System;


public class BossController : BaseEnemyController, ITriggerCheck, IEnemyBaseController
{
    #region Fields and Components

    private BossModel model;
    private BossView view;
    private Rigidbody rb;
    private NavMeshAgent _navMeshAgent;


    public Rigidbody Rb => rb;

    public Transform firePoint;
    public GameObject aggroChecksObject;

    private bool isShieldActive;
    private int pillarsDestroyed = 0;
    //private bool shieldActive = true;

    [SerializeField] private List<Pillar> pillars;
    [SerializeField] private List<Transform> pillarSpawnPoints;
    [SerializeField] private List<Transform> bossSpawnPoints;
    [SerializeField] private Transform bossTransform;
    [SerializeField] private GameObject torretPrefab;
    [SerializeField] private List<Transform> torretSpawnPoints;
    private int bossRespawnIndex = 0;

    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldEffectDuration = 1f;
    //[SerializeField] private float timeToReactivatePillars = 5f;
    //[SerializeField] private float pushDistance = 5f;
    private Collider shieldCollider;


    [SerializeField] private BossUIController bossUIController;

    private AudioSource audioSource;
    [SerializeField] private AudioClip deathClip;


    #endregion

    #region State Machine and FSM

    public EnemyStateMachine<BaseEnemyController> fsm { get; set; }

    public EnemyIdleState IdleState { get; set; }
    public EnemyDeadState DeadState { get; set; }
    public EnemyAttackState AttackState { get; set; }

    [SerializeField] private EnemyAttackSOBase currentAttackSO;
    public override EnemyAttackSOBase CurrentAttackSO => currentAttackSO;

    public enum InitialState
    {
        Attack,
        Idle,
        Dead
    }

    public InitialState initialState = InitialState.Idle;

    #endregion

    #region ScriptableObjects & Behaviors

    [System.Serializable]
    public class AttackPhase
    {
        [Range(0f, 1f)]
        public float healthThreshold;
        public EnemyAttackSOBase attackSO;
    }

    [SerializeField] public List<AttackPhase> attackPhases;

    [Header("FSM-Behaviour ScriptableObjects")]
    [SerializeField] private EnemyIdleSOBase EnemyIdleSOBase;
    [SerializeField] private EnemyDeadSOBase EnemyDeadSOBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; private set; }
    public EnemyDeadSOBase EnemyDeadBaseInstance { get; private set; }


    #endregion

    #region IEnemyBaseController Interface Stubs

    public bool isAggroed { get; set; }
    public bool isWhitinCombatRadius { get; set; }

    public EnemyChaseState ChaseState => throw new System.NotImplementedException();
    public EnemyPatrolState PatrolState => throw new System.NotImplementedException();
    public EnemySearchState SearchState => throw new System.NotImplementedException();
    public EnemyStunnedState StunnedState => throw new System.NotImplementedException();
    public EnemyEscapeState EscapeState => throw new System.NotImplementedException();
    public EnemyHurtState HurtState => throw new System.NotImplementedException();
    public EnemyDefendState DefendState => throw new System.NotImplementedException();

    public override float MaxHealth => throw new System.NotImplementedException();
    public override float CurrentHealth => throw new System.NotImplementedException();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleSOBase);
        EnemyDeadBaseInstance = Instantiate(EnemyDeadSOBase);

        model = GetComponent<BossModel>();
        view = GetComponent<BossView>();
        rb = GetComponent<Rigidbody>();

        fsm = new EnemyStateMachine<BaseEnemyController>();

        IdleState = new EnemyIdleState(this, fsm, EnemyIdleBaseInstance);
        DeadState = new EnemyDeadState(this, fsm, EnemyDeadBaseInstance);

        currentAttackSO = attackPhases[0].attackSO;

        AttackState = new EnemyAttackState(this, fsm);

        shieldCollider = shield.GetComponent<Collider>();
    }

    private void Start()
    {
        foreach (var pillar in pillars)
            pillar.OnPillarDestroyed += HandlePillarDestroyed;

        ActivatePillars();
        ActivateShield();

        foreach (var atk in attackPhases)
            atk.attackSO.Initialize(gameObject, this);

        currentAttackSO = attackPhases[0].attackSO;

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        EnemyDeadBaseInstance.Initialize(gameObject, this);

        InitializeState();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        model.OnHealthChanged += HandleHealthChanged;
        model.OnDeath += HandleDeath;

        isShieldActive = model.statsSO.isShieldActive;


        //UI
        model.OnHealthChanged += currentHealth =>
        {
            bossUIController.UpdateBossHealth(currentHealth, model.MaxHealth);
        };

        for (int i = 0; i < pillars.Count; i++)
        {
            int index = i; // Captura la variable correctamente en el closure
            pillars[index].OnPillarHealthChanged += (current, max) =>
            {
                bossUIController.UpdatePillarHealth(index, current, max);
            };
        }

        // Inicializar los valores actuales
        bossUIController.UpdateBossHealth(model.CurrentHealth, model.MaxHealth);
        for (int i = 0; i < pillars.Count; i++)
        {
            bossUIController.UpdatePillarHealth(i, pillars[i].CurrentHealth, pillars[i].MaxHealth);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        fsm.CurrentState?.FrameUpdate();
        view.PlayMovingAnimation(_navMeshAgent.speed);
        //Debug.Log(fsm.CurrentState);

    }

    #endregion

    #region FSM Logic

    private void InitializeState()
    {
        switch (initialState)
        {
            case InitialState.Idle:
                fsm.Initialize(IdleState);
                break;
            case InitialState.Dead:
                fsm.Initialize(DeadState);
                break;
        }
    }

    #endregion

    #region Attack Logic

    public override void ExecuteAttack(IDamageable target)
    {
        target.TakeDamage(model.statsSO.ProjectileDamage);
    }

    public float GetDamage()
    {
        return model.statsSO.ProjectileDamage;
    }


    #endregion

    #region Health / Damage / Death Handling

    private void HandleHealthChanged(float currentHealth)
    {
        float healthPercent = currentHealth / model.MaxHealth;
        for (int i = attackPhases.Count - 1; i >= 0; i--)
        {
            if (healthPercent <= attackPhases[i].healthThreshold)
            {
                if (attackPhases[i].attackSO != currentAttackSO)
                {
                    currentAttackSO?.DoExitLogic();
                    currentAttackSO = attackPhases[i].attackSO;
                    currentAttackSO?.DoEnterLogic();

                    ReactivateShieldRoutine();
                }
                break;
            }
        }

        view.PlayDamageAnimation();
    }

    private void HandleDeath(BossModel enemy)
    {
        fsm.ChangeState(DeadState);
    }

    public void PlayAudioDead()
    {
        audioSource.PlayOneShot(deathClip);

    }

    #endregion

    #region Aggro / Combat Control

    public void SetAggroChecksEnabled(bool enabled)
    {
        aggroChecksObject.SetActive(enabled);
    }

    public void SetAggroStatus(bool IsAggroed)
    {
        isAggroed = IsAggroed;
    }

    public void SetCombatRadiusBool(bool IsWhitinCombatRadius)
    {
        isWhitinCombatRadius = IsWhitinCombatRadius;
    }

    public bool GetShield()
    {
        return isShieldActive;
    }

    public void SetShield(bool isOn)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Pillar / Shield Logic

    private void HandlePillarDestroyed(Pillar pillar)
    {
        pillarsDestroyed++;

        if (pillarsDestroyed >= pillars.Count)
        {
            StartCoroutine(DeactivateShieldRoutine());
        }
    }

    private IEnumerator DeactivateShieldRoutine()
    {
        TriggerShieldEffect();
        yield return new WaitForSeconds(shieldEffectDuration);

        shield.SetActive(false);
        //shieldActive = false;

    }

    private void ReactivateShieldRoutine()
    {
        BossRespawn();
        TorretsRespawn();
        ReactivatePillars();
        TriggerShieldEffect();
        ActivateShield();
    }

    private void TriggerShieldEffect()
    {
        StartCoroutine(ShieldBlinkEffect());
    }

    private IEnumerator ShieldBlinkEffect()
    {
        Renderer shieldRenderer = shield.GetComponent<Renderer>();
        Material mat = shieldRenderer.material;

        mat.EnableKeyword("_EMISSION");

        float duration = shieldEffectDuration;
        float elapsed = 0f;
        float blinkSpeed = 10f;
        Color baseColor = Color.white;
        float emissionStrength = 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            Color emissionColor = baseColor * (blink * emissionStrength);
            mat.SetColor("_EmissionColor", emissionColor);

            yield return null;
        }

        mat.SetColor("_EmissionColor", baseColor * 0f);
    }

    private void ActivateShield()
    {
        shield.SetActive(true);
        //shieldActive = true;

        Transform playerTransform = PlayerHelper.GetPlayer().transform;
        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        SphereCollider shieldCollider = shield.GetComponent<SphereCollider>();

        if (playerTransform == null || shieldCollider == null)
            return;

        //Convertir centro local del SphereCollider a mundo
        Vector3 shieldCenter = shield.transform.TransformPoint(shieldCollider.center);
        float shieldRadius = shieldCollider.radius * shield.transform.lossyScale.x; 

        Vector3 toPlayer = playerTransform.position - shieldCenter;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        if (distance < shieldRadius)
        {
            Vector3 pushDirection = toPlayer.normalized;
            float pushDistance = shieldRadius - distance + 1f; 

            Vector3 newPosition = playerTransform.position + pushDirection * pushDistance;

            if (playerRb != null)
                playerRb.MovePosition(newPosition);
            else
                playerTransform.position = newPosition;
        }
    }

    private void BossRespawn()
    {
        if (bossSpawnPoints.Count == 0) return;

        if (bossRespawnIndex >= bossSpawnPoints.Count)
        {
            bossRespawnIndex = 0;
        }

        
            _navMeshAgent.enabled = false;

        bossTransform.position = bossSpawnPoints[bossRespawnIndex].position;
        bossTransform.rotation = bossSpawnPoints[bossRespawnIndex].rotation;

      
            _navMeshAgent.enabled = true;

        bossRespawnIndex++;

    }

    private void TorretsRespawn()
    {
        List<Transform> availablePositions = new List<Transform>(torretSpawnPoints);

        int torretCount = 2; 
        for (int i = 0; i < torretCount; i++)
        {
            if (availablePositions.Count == 0)
                break; 

            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            Transform selectedPosition = availablePositions[randomIndex];

            Instantiate(torretPrefab, selectedPosition.position, selectedPosition.rotation);

            availablePositions.RemoveAt(randomIndex);
        }

    }

    private void ActivatePillars()
    {
        //copia temporal de las posiciones disponibles
        List<Transform> availablePositions = new List<Transform>(pillarSpawnPoints);

        foreach (var pillar in pillars)
        {
            //pos aleatoria
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            Transform selectedPosition = availablePositions[randomIndex];

            //colocamos el pilar en esa pos
            pillar.ResetPillar(selectedPosition);

            //eliminamos esa pos de las disponibles
            availablePositions.RemoveAt(randomIndex);
        }

        pillarsDestroyed = 0;
    }

    private void ReactivatePillars()
    {
        ActivatePillars();
    }

    #endregion
}
