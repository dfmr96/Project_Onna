using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseEnemyController, ITriggerCheck, IEnemyBaseController
{
    private EnemyModel model;
    private EnemyView view;
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    private NavMeshAgent _navMeshAgent;

    public Transform firePoint;

    public GameObject shieldObject;
    public GameObject aggroChecksObject;

    private bool isShieldActive;

    public bool isAggroed { get; set; }
    public bool isWhitinCombatRadius { get; set; }

    public EnemyStateMachine<BaseEnemyController> fsm { get; private set; }

    // Estados de la FSM
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemySearchState SearchState { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyStunnedState StunnedState { get; set; }
    public EnemyDeadState DeadState { get; set; }
    public EnemyEscapeState EscapeState { get; set; }
    public EnemyHurtState HurtState { get; set; }
    public EnemyDefendState DefendState { get; set; }

    public enum InitialState
    {
        Patrol, Chase, Attack, Search, Idle, Stunned, Dead, Escape, Defend
    }

    public InitialState initialState = InitialState.Patrol;

    [System.Serializable]
    public class AttackPhase
    {
        [Range(0f, 1f)]
        public float healthThreshold;
        public EnemyAttackSOBase attackSO;
    }

    [SerializeField] public List<AttackPhase> attackPhases;
    public EnemyAttackSOBase currentAttackSO { get; private set; }

    // ScriptableObject behaviors
    [Header("FSM-Behaviour ScriptableObjects")]
    [SerializeField] private EnemyIdleSOBase EnemyIdleSOBase;
    [SerializeField] private EnemyChaseSOBase EnemyChaseSOBase;
    [SerializeField] private EnemyDeadSOBase EnemyDeadSOBase;
    [SerializeField] private EnemyPatrolSOBase EnemyPatrolSOBase;
    [SerializeField] private EnemyStunnedSOBase EnemyStunnedSOBase;
    [SerializeField] private EnemyEscapeSOBase EnemyEscapeSOBase;
    [SerializeField] private EnemyHurtSOBase EnemyHurtSOBase;
    [SerializeField] private EnemyDefendSOBase EnemyDefendSOBase;
    [SerializeField] private EnemySearchSOBase EnemySearchSOBase;

    // Instancias
    public EnemyIdleSOBase EnemyIdleBaseInstance { get; private set; }
    public EnemySearchSOBase EnemySearchBaseInstance { get; private set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; private set; }
    public EnemyDeadSOBase EnemyDeadBaseInstance { get; private set; }
    public EnemyPatrolSOBase EnemyPatrolBaseInstance { get; private set; }
    public EnemyStunnedSOBase EnemyStunnedBaseInstance { get; private set; }
    public EnemyEscapeSOBase EnemyEscapeBaseInstance { get; private set; }
    public EnemyHurtSOBase EnemyHurtBaseInstance { get; private set; }
    public EnemyDefendSOBase EnemyDefendBaseInstance { get; private set; }

    private List<EnemyAttackSOBase> attackPhaseInstances = new();

    void Awake()
    {
        // Instanciar behaviors
        EnemyIdleBaseInstance = Instantiate(EnemyIdleSOBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseSOBase);
        EnemyDeadBaseInstance = Instantiate(EnemyDeadSOBase);
        EnemyPatrolBaseInstance = Instantiate(EnemyPatrolSOBase);
        EnemyStunnedBaseInstance = Instantiate(EnemyStunnedSOBase);
        EnemyEscapeBaseInstance = Instantiate(EnemyEscapeSOBase);
        EnemyHurtBaseInstance = Instantiate(EnemyHurtSOBase);
        EnemyDefendBaseInstance = Instantiate(EnemyDefendSOBase);
        EnemySearchBaseInstance = Instantiate(EnemySearchSOBase);

        foreach (var phase in attackPhases)
        {
            var instance = Instantiate(phase.attackSO);
            attackPhaseInstances.Add(instance);
        }

        model = GetComponent<EnemyModel>();
        view = GetComponent<EnemyView>();
        rb = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        fsm = new EnemyStateMachine<BaseEnemyController>();

        // Inicializaci�n de estados
        PatrolState = new EnemyPatrolState(this, fsm, EnemyPatrolBaseInstance);
        ChaseState = new EnemyChaseState(this, fsm, EnemyChaseBaseInstance);
        IdleState = new EnemyIdleState(this, fsm, EnemyIdleBaseInstance);
        DeadState = new EnemyDeadState(this, fsm, EnemyDeadBaseInstance);
        SearchState = new EnemySearchState(this, fsm, EnemySearchBaseInstance);
        StunnedState = new EnemyStunnedState(this, fsm, EnemyStunnedBaseInstance);
        EscapeState = new EnemyEscapeState(this, fsm, EnemyEscapeBaseInstance);
        HurtState = new EnemyHurtState(this, fsm, EnemyHurtBaseInstance);
        DefendState = new EnemyDefendState(this, fsm, EnemyDefendBaseInstance);

        // El AttackState se construye con el currentAttackSO din�mico
        currentAttackSO = attackPhaseInstances[0];
        AttackState = new EnemyAttackState(this, fsm);
    }

    void Start()
    {
        foreach (var atk in attackPhaseInstances)
            atk.Initialize(gameObject, this);

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyDeadBaseInstance.Initialize(gameObject, this);
        EnemyPatrolBaseInstance.Initialize(gameObject, this);
        EnemyStunnedBaseInstance.Initialize(gameObject, this);
        EnemyEscapeBaseInstance.Initialize(gameObject, this);
        EnemyHurtBaseInstance.Initialize(gameObject, this);
        EnemyDefendBaseInstance.Initialize(gameObject, this);
        EnemySearchBaseInstance.Initialize(gameObject, this);

        model.OnHealthChanged += HandleHealthChanged;
        model.OnDeath += HandleDeath;

        isShieldActive = model.statsSO.isShieldActive;
        SetShield(isShieldActive);

        InitializeState();


}

void Update()
    {
        view.PlayMovingAnimation(_navMeshAgent.speed);
        fsm.CurrentState?.FrameUpdate();

        //Debug.Log("ESTADO: " + fsm.CurrentState);

        model.statsSO.currentState = fsm.CurrentState.ToString();

    


    }

    private void InitializeState()
    {
        switch (initialState)
        {
            case InitialState.Patrol: fsm.Initialize(PatrolState); break;
            case InitialState.Chase: fsm.Initialize(ChaseState); break;
            case InitialState.Attack: fsm.Initialize(AttackState); break;
            case InitialState.Search: fsm.Initialize(SearchState); break;
            case InitialState.Idle: fsm.Initialize(IdleState); break;
            case InitialState.Stunned: fsm.Initialize(StunnedState); break;
            case InitialState.Dead: fsm.Initialize(DeadState); break;
            case InitialState.Escape: fsm.Initialize(EscapeState); break;
            case InitialState.Defend: fsm.Initialize(DefendState); break;
        }
    }

    public override void ExecuteAttack(IDamageable target)
    {
        target.TakeDamage(model.currentDamage);

        //Si el que ataque es una variante verde aplica veneno
        if(model.variantSO.variantType == EnemyVariantType.Green)
        {
            target.ApplyDebuffDoT(model.variantSO.dotDuration, model.variantSO.dotDamage);
        }
    }

    public float GetDamage() => model.currentDamage;

    //public void DoAttack(IDamageable target)
    //{
    //    target.TakeDamage(GetDamage());
    //    Debug.Log("Da�o hecho por el estado Melee");
    //}

    private void HandleHealthChanged(float currentHealth) => view.HandleDamage();

    private void HandleDeath(EnemyModel enemy) => fsm.ChangeState(DeadState);

    public void SetAggroChecksEnabled(bool enabled) => aggroChecksObject.SetActive(enabled);
    public void SetAggroStatus(bool value) => isAggroed = value;
    public void SetCombatRadiusBool(bool value) => isWhitinCombatRadius = value;

    public void SetShield(bool isOn)
    {
        isShieldActive = isOn;
        shieldObject.SetActive(isOn);
    }

    public bool GetShield() => isShieldActive;

    public override float MaxHealth => model.statsSO.MaxHealth;
    public override float CurrentHealth => model.CurrentHealth;
    public override EnemyAttackSOBase CurrentAttackSO => currentAttackSO;


    //void OnGUI()
    //{
    //    if (fsm?.CurrentState != null)
    //    {
    //        GUIStyle style = new GUIStyle();
    //        style.fontSize = 16;
    //        style.normal.textColor = Color.white;

    //        GUI.Label(new Rect(10, 200, 400, 30), $"Estado FSM: {fsm.CurrentState.GetType().Name}", style);
    //    }
    //}
}
