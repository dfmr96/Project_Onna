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

    [System.Serializable]
    public class AttackPhase
    {
        [Range(0f, 1f)]
        public float healthThreshold;
        public EnemyAttackSOBase attackSO;
    }

    [SerializeField]
    public List<AttackPhase> attackPhases;
    public EnemyAttackSOBase currentAttackSO;

    [Header("Pillars Stats")]
    [SerializeField] private List<Pillar> pillars;
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldEffectDuration = 1f;
    [SerializeField] private float timeToReactivatePillars = 5f;


    private int pillarsDestroyed = 0;
    private bool shieldActive = true;


    public Transform firePoint;
    public GameObject shieldObject;
    private bool isShieldActive;
    public GameObject aggroChecksObject;


    public bool isAggroed { get; set; }
    public bool isWhitinCombatRadius { get; set; }

    #region State Machine Variables

    public EnemyStateMachine<BaseEnemyController> fsm { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyDeadState DeadState { get; set; }





    public enum InitialState
    {
        Attack,
        Idle,
        Dead
    }

    public InitialState initialState = InitialState.Idle;


    #endregion


    #region Behaviours
    //Behaviour
    [Header("FSM-Behaviour ScriptableObjects")]
    [SerializeField] private EnemyIdleSOBase EnemyIdleSOBase;
    [SerializeField] private EnemyDeadSOBase EnemyDeadSOBase;




    public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    public EnemyDeadSOBase EnemyDeadBaseInstance { get; set; }

    private List<EnemyAttackSOBase> attackPhaseInstances = new();




    #endregion

    void Awake()
    {
        //Behaviour
        EnemyIdleBaseInstance = Instantiate(EnemyIdleSOBase);
        EnemyDeadBaseInstance = Instantiate(EnemyDeadSOBase);


        foreach (var phase in attackPhases)
        {
            var instance = Instantiate(phase.attackSO);
            attackPhaseInstances.Add(instance);
        }

        model = GetComponent<BossModel>();
        view = GetComponent<BossView>();
        rb = GetComponent<Rigidbody>();

        fsm = new EnemyStateMachine<BaseEnemyController>();


        //IdleState = new EnemyIdleState(this, fsm, EnemyIdleSOBase);
        //DeadState = new EnemyDeadState(this, fsm, EnemyDeadSOBase);
       



    }

    public Rigidbody Rb => rb;

    private void Start()
    {
        foreach (var pillar in pillars)
        {
            pillar.OnPillarDestroyed += HandlePillarDestroyed;
        }

        ActivatePillars();
        ActivateShield();

        for (int i = 0; i < attackPhaseInstances.Count; i++)
        {
            //attackPhaseInstances[i].Initialize(gameObject, this);
        }

        // Seleccioná el primero por defecto
        currentAttackSO = attackPhaseInstances[0];

        //Behaviour
        //EnemyIdleBaseInstance.Initialize(gameObject, this);
        //EnemyDeadBaseInstance.Initialize(gameObject, this);
    




        InitializeState();



        _navMeshAgent = GetComponent<NavMeshAgent>();


        model.OnHealthChanged += HandleHealthChanged;
        model.OnDeath += HandleDeath;

        isShieldActive = model.statsSO.isShieldActive;
        SetShield(isShieldActive);
    }

    private void Update()
    {
        //fsm.CurrentEnemyState.FrameUpdate();

        //Debug.Log(fsm.CurrentEnemyState);

        //Animacion de Movimiento
        view.PlayMovingAnimation(_navMeshAgent.speed);
    }

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

    //Realiza el ataque desde el eventtrigger de la animacion
    public void ExecuteAttack(IDamageable target)
    {

        target.TakeDamage(model.statsSO.AttackDamage);


    }

    public float GetDamage()
    {
        return model.statsSO.AttackDamage;
    }
    public void DoAttack(IDamageable target)
    {
        target.TakeDamage(GetDamage());
        Debug.Log("Daño hecho por el estado Melee");
    }

    private void HandleHealthChanged(float currentHealth)
    {
        //float healthPercentage = currentHealth / model.statsSO.MaxHealth;

        //Cuando lo hieren pasa a Hurt
        //fsm.ChangeState(HurtState);
        float healthPercent = currentHealth / model.MaxHealth;

        for (int i = 0; i < attackPhases.Count; i++)
        {
            if (healthPercent <= attackPhases[i].healthThreshold)
            {
                currentAttackSO = attackPhaseInstances[i];
                break;
            }
        }

        view.PlayDamageAnimation();
    }

    private void HandleDeath(BossModel enemy)
    {
        fsm.ChangeState(DeadState);
    }

    public void SetAggroChecksEnabled(bool enabled)
    {
        if (enabled)
        {

            aggroChecksObject.SetActive(true);
        }
        else
        {
            aggroChecksObject.SetActive(false);

        }
    }

    public void SetAggroStatus(bool IsAggroed)
    {
        isAggroed = IsAggroed;
    }

    public void SetCombatRadiusBool(bool IsWhitinCombatRadius)
    {
        isWhitinCombatRadius = IsWhitinCombatRadius;
    }

    public void SetShield(bool isGod)
    {
        isShieldActive = isGod;

        if (isGod)
        {
            shieldObject.SetActive(true);
        }
        else
        {
            shieldObject.SetActive(false);
        }
    }

    public bool GetShield()
    {
        return isShieldActive;
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

        //tiempo para revivir los pilares
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
        float blinkSpeed = 10f; //cuantos parpadeos por segundo
        Color baseColor = Color.white; //Color del escudo
        float emissionStrength = 2f;  //Intensidad del brillo

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
            pillar.ResetPillar(); //Restaura salud y estado visual
        }

        pillarsDestroyed = 0;
    }

    private void ReactivatePillars()
    {
        ActivatePillars();
    }




}
