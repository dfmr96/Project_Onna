using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Player;


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
    private bool shieldActive = true;

    [SerializeField] private List<Pillar> pillars;
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldEffectDuration = 1f;
    [SerializeField] private float timeToReactivatePillars = 5f;
    [SerializeField] private float pushDistance = 5f;
    private Collider shieldCollider;


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


    }

    private void Update()
    {
        fsm.CurrentState?.FrameUpdate();
        view.PlayMovingAnimation(_navMeshAgent.speed);
        Debug.Log(fsm.CurrentState);
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
        target.TakeDamage(model.statsSO.AttackDamage);
    }

    public float GetDamage()
    {
        return model.statsSO.AttackDamage;
    }

    //public void DoAttack(IDamageable target)
    //{
    //    target.TakeDamage(GetDamage());
    //    Debug.Log("Daño hecho por el estado Melee");
    //}

    #endregion

    #region Health / Damage / Death Handling

    private void HandleHealthChanged(float currentHealth)
    {
        float healthPercent = currentHealth / model.MaxHealth;
        Debug.Log("Vida: " + healthPercent);
        for (int i = attackPhases.Count - 1; i >= 0; i--)
        {
            if (healthPercent <= attackPhases[i].healthThreshold)
            {
                if (attackPhases[i].attackSO != currentAttackSO)
                {
                    Debug.Log("Entró en fase: " + i);
                    currentAttackSO?.DoExitLogic();
                    currentAttackSO = attackPhases[i].attackSO;
                    //currentAttackSO?.DoEnterLogic(); // activar el nuevo ataque
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
        shieldActive = false;

        yield return new WaitForSeconds(timeToReactivatePillars);
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
        shieldActive = true;

        Transform playerTransform = PlayerHelper.GetPlayer().transform;
        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        SphereCollider shieldCollider = shield.GetComponent<SphereCollider>();

        if (playerTransform == null || shieldCollider == null)
            return;

        //Convertir centro local del SphereCollider a mundo
        Vector3 shieldCenter = shield.transform.TransformPoint(shieldCollider.center);
        float shieldRadius = shieldCollider.radius * shield.transform.lossyScale.x; 

        Vector3 toPlayer = playerTransform.position - shieldCenter;
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

    private void ActivatePillars()
    {
        foreach (var pillar in pillars)
        {
            pillar.ResetPillar();
        }

        pillarsDestroyed = 0;
    }

    private void ReactivatePillars()
    {
        ActivatePillars();
    }

    #endregion
}
