using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Attack-Charging attack Tank type", menuName = "Enemy Logic/Attack Logic/Charging attack Tank type")]
public class EnemyAttackCharge : EnemyAttackSOBase
{
    [Header("Tank Settings")]
    [SerializeField] private float punchChargeRadius = 1f;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float maxChargeTime = 1f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float postChargeDelay = 0.5f;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private float stopDistanceFromPlayer = 1.5f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particlePunchAttack;
    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] private float downOffset = 1f;
    private ParticleSystem damageParticlesInstance;

    private bool isPreparingCharge = false;
    private bool isChargeActive = false;
    private bool isPostCharging = false;

    private float chargeTimer = 0f;
    private float postChargeTimer = 0f;
    private Vector3 chargeDirection;
    private Vector3 chargeEndPoint;

    private bool _hasAttackedOnce = false;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _hasAttackedOnce = false;
        isPreparingCharge = false;
        isChargeActive = false;
        isPostCharging = false;

        _enemyView.OnAttackImpact += OnAttackImpact;
        _enemyView.OnAttackStarted += OnMeleeAttack;

        damageParticlesInstance = Instantiate(
                   particlePunchAttack,
                   _enemyView.PunchPoint.position,
                   Quaternion.identity
               );

        damageParticlesInstance.Stop();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _enemyView.OnAttackImpact -= OnAttackImpact;
        _enemyView.OnAttackStarted -= OnMeleeAttack;

        Destroy(damageParticlesInstance.gameObject);

        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        _timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Post-charge
        if (isPostCharging)
        {
            postChargeTimer += Time.deltaTime;
            if (postChargeTimer >= postChargeDelay)
            {
                isPostCharging = false;
                _timer = 0f;
                enemy.SetShield(true);
            }
            return;
        }

        // Player demasiado lejos
        if (distanceToPlayer > _distanceToCountExit)
        {
            EndAttackAnimations();
            enemy.SetShield(true);
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        // Charge activo
        if (isChargeActive)
        {
            chargeTimer += Time.deltaTime;

            // Mover enemigo
            //_rb.position += chargeDirection * chargeSpeed * Time.deltaTime;
            Vector3 newPos = _rb.position + chargeDirection * chargeSpeed * Time.deltaTime;

            // si ya pasó el endPoint, clávalo en el endPoint y terminá el charge
            if (Vector3.Dot(chargeEndPoint - transform.position, chargeDirection) <= 0f)
            {
                _rb.position = chargeEndPoint;
                EndCharge();
            }
            else
            {
                _rb.position = newPos;
            }

          
                // Chequear colisión con player
                Collider[] hits = Physics.OverlapSphere(_enemyView.PunchPoint.position, punchChargeRadius, damageableLayers);
                foreach (var hit in hits)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        enemy.ExecuteAttack(damageable);
                        EndCharge(); // Frenar charge al golpear
                        break;
                    }
                }
            

            // Limitar duración máxima
            if (chargeTimer >= maxChargeTime)
                EndCharge();

            return;
        }

        // Espera entre ataques
        if (_hasAttackedOnce)
        {
            if (_timer >= _enemyModel.currentAttackTimeRate)
            {
                _hasAttackedOnce = false;
                _timer = 0f;
            }
            return;
        }

        bool canSeePlayer = CanChargeToPlayer(out Vector3 dir);

        // Si no hay línea de visión, aunque esté en rango de ataque, retrocede a chase
        if (!canSeePlayer)
        {
            EndAttackAnimations();
            enemy.SetShield(true);
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        if (_timer >= _enemyModel.statsSO.AttackInitialDelay)
        {
            // Si hay línea de visión, decidí si hacer melee o charge
            if (distanceToPlayer <= meleeRange)
            {
                StartMeleeAttack();
            }
            else
            {
                chargeDirection = dir;
                PrepareCharge();
            }
        }

 
    }

    private void StartMeleeAttack()
    {
        _enemyView.PlayAttackAnimation(false);
        _enemyView.PlayMeleeAttackAnimation(true);
        _hasAttackedOnce = true;
        _timer = 0f;
    }

    private void PrepareCharge()
    {
        // Animación pre-attack
        _enemyView.PlayAttackAnimation(true);
        _enemyView.PlayMeleeAttackAnimation(false);

        isPreparingCharge = true;
        _hasAttackedOnce = true;
        _timer = 0f;

        // Detener NavMeshAgent mientras se prepara
        _navMeshAgent.isStopped = true;
    }

    // Animation Event
    private void OnAttackImpact()
    {
        if (!isPreparingCharge) return;

        isPreparingCharge = false;
        isChargeActive = true;
        chargeTimer = 0f;


        // Definir dirección y endpoint
        chargeDirection = (playerTransform.position - transform.position).normalized;
        //chargeEndPoint = transform.position + chargeDirection * 5f;

        // calculamos un "endPoint" que queda a cierta distancia del player
        Vector3 toPlayer = playerTransform.position - transform.position;
        float totalDistance = Mathf.Max(0f, toPlayer.magnitude - stopDistanceFromPlayer);
        chargeEndPoint = transform.position + chargeDirection * totalDistance;


        // Apagar NavMesh mientras dura el charge
        _navMeshAgent.isStopped = true;
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        // PARTICULA
        Vector3 forward = _enemyView.PunchPoint.forward;
        forward.y = 0f; // aplastamos la componente vertical
        forward.Normalize(); // aseguramos que sea unitario

        Quaternion flatRotation = Quaternion.LookRotation(forward, Vector3.up);
        Vector3 spawnPos = _enemyView.PunchPoint.position
                         + forward * forwardOffset
                         + Vector3.down * downOffset;
        damageParticlesInstance.transform.SetPositionAndRotation(
            spawnPos,
            flatRotation
        );

        damageParticlesInstance.Clear();
        damageParticlesInstance.Play();
    }

    private void EndCharge()
    {
        isChargeActive = false;
        isPostCharging = true;
        postChargeTimer = 0f;



        _enemyView.PlayAttackAnimation(false);
        _enemyView.PlayMeleeAttackAnimation(false);

        // Restaurar NavMesh
        _navMeshAgent.isStopped = false;
        _navMeshAgent.updatePosition = true;
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.Warp(_rb.position); // actualizar posición real

        enemy.SetShield(false);
        enemy.fsm.ChangeState(enemy.IdleState);

    }

    private void OnMeleeAttack()
    {
        Vector3 impactPos = _enemyView.PunchPoint.position;
        Collider[] hits = Physics.OverlapSphere(impactPos, punchChargeRadius, damageableLayers);
        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
                enemy.ExecuteAttack(damageable);
        }

        // PARTICULA
        Vector3 forward = _enemyView.PunchPoint.forward;
        forward.y = 0f; // aplastamos la componente vertical
        forward.Normalize(); // aseguramos que sea unitario

        Quaternion flatRotation = Quaternion.LookRotation(forward, Vector3.up);

        Vector3 spawnPos = _enemyView.PunchPoint.position + Vector3.down * downOffset;

        damageParticlesInstance.transform.SetPositionAndRotation(
            spawnPos,
            flatRotation
        );

        damageParticlesInstance.Clear();
        damageParticlesInstance.Play();
    }

    private void EndAttackAnimations()
    {
        _enemyView.PlayAttackAnimation(false);
        _enemyView.PlayMeleeAttackAnimation(false);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        isPreparingCharge = false;
        isChargeActive = false;
        isPostCharging = false;
        chargeTimer = 0f;
        postChargeTimer = 0f;
        chargeDirection = Vector3.zero;
        _hasAttackedOnce = false;

        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
    }

    private bool CanChargeToPlayer(out Vector3 direction)
    {
        direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 origin = transform.position + Vector3.up * 1f;

        // radio del "chequeo frontal"
        float rayRadius = 0.5f;

        // hacemos un SphereCast en dirección al player
        if (Physics.SphereCast(origin, rayRadius, direction, out RaycastHit hit, distance))
        {
            // si choca algo que NO es el player  no puede cargar
            if (hit.transform != playerTransform)
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                return false;
            }
        }

        Debug.DrawLine(origin, playerTransform.position, Color.green);
        return true;
    }

}
