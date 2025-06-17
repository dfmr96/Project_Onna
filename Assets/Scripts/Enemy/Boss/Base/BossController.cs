using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public class BossController : MonoBehaviour, ITriggerCheck
public class BossController : MonoBehaviour

{

    private BossModel model;
    private BossView view;
    private Rigidbody rb;

    private NavMeshAgent _navMeshAgent;



    [SerializeField] private List<Pillar> pillars;
    [SerializeField] private GameObject shield;   
    [SerializeField] private float shieldEffectDuration = 1f;

    private int pillarsDestroyed = 0;
    private bool shieldActive = true;













    //public Transform firePoint;
    //public GameObject shieldObject;
    //private bool isShieldActive;
    //public GameObject aggroChecksObject;


    //public bool isAggroed { get; set; }
    //public bool isWhitinCombatRadius { get; set; }

    #region State Machine Variables

    public EnemyStateMachine fsm { get; set; }
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
        Patrol,
        Chase,
        Attack,
        Search,
        Idle,
        Stunned,
        Dead,
        Escape,
        Defend
    }

    public InitialState initialState = InitialState.Patrol;


    #endregion


    //#region Behaviours
    ////Behaviour
    //[Header("FSM-Behaviour ScriptableObjects")]
    //[SerializeField] private EnemyIdleSOBase EnemyIdleSOBase;
    //[SerializeField] private EnemyAttackSOBase EnemyAttackSOBase;
    //[SerializeField] private EnemyChaseSOBase EnemyChaseSOBase;
    //[SerializeField] private EnemyDeadSOBase EnemyDeadSOBase;
    //[SerializeField] private EnemyPatrolSOBase EnemyPatrolSOBase;
    //[SerializeField] private EnemyStunnedSOBase EnemyStunnedSOBase;
    //[SerializeField] private EnemyEscapeSOBase EnemyEscapeSOBase;
    //[SerializeField] private EnemyHurtSOBase EnemyHurtSOBase;
    //[SerializeField] private EnemyDefendSOBase EnemyDefendSOBase;




    //public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    //public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
    //public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    //public EnemyDeadSOBase EnemyDeadBaseInstance { get; set; }
    //public EnemyPatrolSOBase EnemyPatrolBaseInstance { get; set; }
    //public EnemyStunnedSOBase EnemyStunnedBaseInstance { get; set; }
    //public EnemyEscapeSOBase EnemyEscapeBaseInstance { get; set; }
    //public EnemyHurtSOBase EnemyHurtBaseInstance { get; set; }
    //public EnemyDefendSOBase EnemyDefendBaseInstance { get; set; }




    //#endregion

    //    void Awake()
    //    {
    //        //Behaviour
    //        EnemyIdleBaseInstance = Instantiate(EnemyIdleSOBase);
    //        EnemyAttackBaseInstance = Instantiate(EnemyAttackSOBase);
    //        EnemyChaseBaseInstance = Instantiate(EnemyChaseSOBase);
    //        EnemyDeadBaseInstance = Instantiate(EnemyDeadSOBase);
    //        EnemyPatrolBaseInstance = Instantiate(EnemyPatrolSOBase);
    //        EnemyStunnedBaseInstance = Instantiate(EnemyStunnedSOBase);
    //        EnemyEscapeBaseInstance = Instantiate(EnemyEscapeSOBase);
    //        EnemyHurtBaseInstance = Instantiate(EnemyHurtSOBase);
    //        EnemyDefendBaseInstance = Instantiate(EnemyDefendSOBase);





    //        model = GetComponent<EnemyModel>();
    //        view = GetComponent<EnemyView>();
    //        rb = GetComponent<Rigidbody>();

    //        fsm = new EnemyStateMachine();

    //        PatrolState = new EnemyPatrolState(this, fsm);
    //        ChaseState = new EnemyChaseState(this, fsm);
    //        AttackState = new EnemyAttackState(this, fsm);
    //        SearchState = new EnemySearchState(this, fsm);
    //        StunnedState = new EnemyStunnedState(this, fsm);
    //        IdleState = new EnemyIdleState(this, fsm);
    //        DeadState = new EnemyDeadState(this, fsm);
    //        EscapeState = new EnemyEscapeState(this, fsm);
    //        HurtState = new EnemyHurtState(this, fsm);
    //        DefendState = new EnemyDefendState(this, fsm);



    //    }

    //    public Rigidbody Rb => rb;

    //    private void Start()
    //    {


    //        //Behaviour
    //        EnemyIdleBaseInstance.Initialize(gameObject, this);
    //        EnemyAttackBaseInstance.Initialize(gameObject, this);
    //        EnemyChaseBaseInstance.Initialize(gameObject, this);
    //        EnemyDeadBaseInstance.Initialize(gameObject, this);
    //        EnemyPatrolBaseInstance.Initialize(gameObject, this);
    //        EnemyStunnedBaseInstance.Initialize(gameObject, this);
    //        EnemyEscapeBaseInstance.Initialize(gameObject, this);
    //        EnemyHurtBaseInstance.Initialize(gameObject, this);
    //        EnemyDefendBaseInstance.Initialize(gameObject, this);




    //        InitializeState();



    //        _navMeshAgent = GetComponent<NavMeshAgent>();


    //        model.OnHealthChanged += HandleHealthChanged;
    //        model.OnDeath += HandleDeath;

    //        isShieldActive = model.statsSO.isShieldActive;
    //        SetShield(isShieldActive);
    //    }

    //    private void Update()
    //    {
    //        fsm.CurrentEnemyState.FrameUpdate();

    //        Debug.Log(fsm.CurrentEnemyState);

    //        //Animacion de Movimiento
    //        view.PlayMovingAnimation(_navMeshAgent.speed);
    //    }

    //    private void InitializeState()
    //    {
    //        switch (initialState)
    //        {
    //            case InitialState.Patrol:
    //                fsm.Initialize(PatrolState);
    //                break;
    //            case InitialState.Chase:
    //                fsm.Initialize(ChaseState);
    //                break;
    //            case InitialState.Attack:
    //                fsm.Initialize(AttackState);
    //                break;
    //            case InitialState.Search:
    //                fsm.Initialize(SearchState);
    //                break;
    //            case InitialState.Idle:
    //                fsm.Initialize(IdleState);
    //                break;
    //            case InitialState.Stunned:
    //                fsm.Initialize(StunnedState);
    //                break;
    //            case InitialState.Dead:
    //                fsm.Initialize(DeadState);
    //                break;
    //            case InitialState.Escape:
    //                fsm.Initialize(EscapeState);
    //                break;


    //        }
    //    }

    //    //Realiza el ataque desde el eventtrigger de la animacion
    //    public void ExecuteAttack(IDamageable target)
    //    {

    //        target.TakeDamage(model.statsSO.AttackDamage);


    //    }

    //    public float GetDamage()
    //    {
    //        return model.statsSO.AttackDamage;
    //    }
    //    public void DoAttack(IDamageable target)
    //    {
    //        target.TakeDamage(GetDamage());
    //        Debug.Log("Daño hecho por el estado Melee");
    //    }

    //    private void HandleHealthChanged(float currentHealth)
    //    {
    //        //float healthPercentage = currentHealth / model.statsSO.MaxHealth;

    //        //Cuando lo hieren pasa a Hurt
    //        //fsm.ChangeState(HurtState);
    //        view.PlayDamageAnimation();
    //    }

    //    private void HandleDeath(EnemyModel enemy)
    //    {
    //        fsm.ChangeState(DeadState);
    //    }

    //    public void SetAggroChecksEnabled(bool enabled)
    //    {
    //        if (enabled) {

    //            aggroChecksObject.SetActive(true);
    //        }
    //        else
    //        {
    //            aggroChecksObject.SetActive(false);

    //        }
    //    }

    //    public void SetAggroStatus(bool IsAggroed)
    //    {
    //        isAggroed = IsAggroed;
    //    }

    //    public void SetCombatRadiusBool(bool IsWhitinCombatRadius)
    //    {
    //        isWhitinCombatRadius = IsWhitinCombatRadius;
    //    }

    //    public void SetShield(bool isGod)
    //    {
    //        isShieldActive = isGod;

    //        if (isGod)
    //        {
    //            shieldObject.SetActive(true);
    //        }
    //        else
    //        {
    //            shieldObject.SetActive(false);
    //        }
    //    }

    //    public bool GetShield()
    //    {
    //        return isShieldActive;
    //    }



    private void Start()
    {
        foreach (var pillar in pillars)
        {
            pillar.OnPillarDestroyed += HandlePillarDestroyed;
        }

        ActivatePillars();
        ActivateShield();
    }

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
        // Efecto de shader o feedback visual
        TriggerShieldEffect();

        yield return new WaitForSeconds(shieldEffectDuration);

        shield.SetActive(false);
        shieldActive = false;

        // Aquí podrías empezar un timer o esperar un evento para revivir los pilares
        yield return new WaitForSeconds(5f); // Por ejemplo, espera 10s
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
        float blinkSpeed = 10f; // cuántos parpadeos por segundo
        Color baseColor = Color.white; // Color del escudo
        float emissionStrength = 2f;  // Intensidad del brillo

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            Color emissionColor = baseColor * (blink * emissionStrength);
            mat.SetColor("_EmissionColor", emissionColor);

            yield return null;
        }

        // Reset después del efecto
        mat.SetColor("_EmissionColor", baseColor * 0f); // apagar
    }


    private void ActivateShield()
    {
        shield.SetActive(true);
        shieldActive = true;
    }

    private void ActivatePillars()
    {
        foreach (var pillar in pillars)
        {
            pillar.ResetPillar(); // Restaura salud y estado visual
        }

        pillarsDestroyed = 0;
    }

    private void ReactivatePillars()
    {
        ActivatePillars();
    }




}
