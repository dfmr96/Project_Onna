using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(fileName = "Attack-Melee Basic", menuName = "Enemy Logic/Attack Logic/Melee Attack")]
public class EnemyAttackMelee : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;
    private float attackRange;
    private bool _canBeStunned = false;
    private bool _attackFinished = true;

    [Header("Melee Settings")]
    [SerializeField] private float punchRadius = 0.5f;
    [SerializeField] private float dashDistance = 1f;

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);



    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        attackRange = _enemyModel.statsSO.AttackRange;


        _enemyModel.OnHealthChanged += HandleHealthChanged;
        _enemyView.OnAttackStarted += OnAttackStarted;
        _enemyView.OnAttackImpact += OnAttackImpact;
        _enemyView.OnAttackCanStun += OnAttackCanStun;
        _enemyView.OnAttackFinished += OnAttackFinished;


        _navMeshAgent.SetDestination(playerTransform.position);
        _hasAttackedOnce = false;
 


        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.ResetPath();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        _enemyView.PlayAttackAnimation(false);

        _enemyModel.OnHealthChanged -= HandleHealthChanged;
        _enemyView.OnAttackStarted -= OnAttackStarted;
        _enemyView.OnAttackImpact -= OnAttackImpact;
        _enemyView.OnAttackCanStun -= OnAttackCanStun;
        _enemyView.OnAttackFinished -= OnAttackFinished;



        _hasAttackedOnce = false;
 

        _navMeshAgent.speed = _enemyModel.currentSpeed;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed;

        _navMeshAgent.isStopped = false;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // Solo después de terminar el ataque evaluamos cambios de estado
        if (_attackFinished)
        {
            if (distanceToPlayer > attackRange)
            {
                _enemyView.PlayAttackAnimation(false);
                enemy.fsm.ChangeState(enemy.SearchState);
                return;
            }
        }


        // === Lógica de ataques ===
        if (!_hasAttackedOnce)
        {
            if (_timer >= _enemyModel.statsSO.AttackInitialDelay)
            {
                Attack();
                _hasAttackedOnce = true;
                _timer = 0f;
            }
        }
        else if (_timer >= _enemyModel.currentAttackTimeRate)
        {
            Attack();
            _timer = 0f;
        }


        // Rotación manual hacia el jugador
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0f; // Opcional: evita que incline la cabeza hacia arriba o abajo
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _enemyModel.statsSO.rotationSpeed * Time.deltaTime);
        }


    }

    private void Attack()
    {
        if (playerTransform == null) return;

           distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= attackRange)
        {
            _enemyView.PlayAttackAnimation(true);
        }
        //else if (_attackFinished && distanceToPlayer >= attackRange) 
        //{
        //    enemy.fsm.ChangeState(enemy.ChaseState);

        //}
    }

    private void OnAttackStarted()
    {
        _attackFinished = false;
    }

    private void OnAttackFinished()
    {
        _attackFinished = true;

    
        //// Retroceder un poco para respetar el attackRange
        //Vector3 directionFromPlayer = (transform.position - playerTransform.position).normalized;
        //float safeDistance = attackRange * 0.8f; // Ajusta si querés un poquito más lejos del jugador

        //Vector3 targetPos = playerTransform.position + directionFromPlayer * safeDistance;

        //// Asegurarse que esté sobre el NavMesh
        //if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        //{
        //    _navMeshAgent.Warp(hit.position);
        //}
    }

    private void OnAttackCanStun()
    {
        _canBeStunned = true;
    }

    private void OnAttackImpact()
    {
        _canBeStunned = false;

        // Avanzar un poquito hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // Calcular nuevo destino dentro del NavMesh
        Vector3 targetPos = transform.position + direction * dashDistance;

        // Asegurarse que esté sobre el NavMesh
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hitN, 1f, NavMesh.AllAreas))
        {
            _navMeshAgent.Warp(hitN.position); // Teletransporta suavemente a esa posición
        }

        Collider[] hits = Physics.OverlapSphere(_enemyView.PunchPoint.position, punchRadius);

 
        foreach (var hit in hits)
        {
            if (hit.transform == playerTransform)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    enemy.ExecuteAttack(damageable);
                }
            }
        }

       
    }

  

    private void HandleHealthChanged(float currentHealth)
    {
        if (_canBeStunned)
        {
            enemy.fsm.ChangeState(enemy.StunnedState);
        }
    }

  
}
