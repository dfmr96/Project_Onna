using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Attack-Charging attack Tank type", menuName = "Enemy Logic/Attack Logic/Charging attack Tank type")]
public class EnemyAttackCharge : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;

    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float postChargeDelay = 0.5f;
    private float postChargeTimer;
    private bool isPostCharging;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private Vector3 chargeDirection;
    private Vector3 chargeEndPoint;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();


        //enemy.SetShield(false);

        _navMeshAgent.SetDestination(playerTransform.position);
        _hasAttackedOnce = false;
        isCharging = false;

        _navMeshAgent.isStopped = true;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();


        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        _timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);




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

        // Si se aleja del rango de ataque cambiar a busqueda
        if (distanceToPlayer > _distanceToCountExit)
        {
            EndAttackAnimations();
            enemy.SetShield(true);
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        if (isCharging)
        {
            chargeTimer += Time.fixedDeltaTime;

        

            // Movimiento suave con física
            _rb.MovePosition(_rb.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);

            distanceToPlayer = Vector3.Distance(_rb.position, playerTransform.position);

            if (distanceToPlayer < 1f || chargeTimer >= chargeDuration)
            {
                EndCharge();
            }

            if (Vector3.Distance(enemy.transform.position, chargeEndPoint) < 0.5f)
            {
                isCharging = false;
                _navMeshAgent.enabled = true; // volver al navmesh
            }
        }

        // Si ya atacó una vez, espera el tiempo entre ataques
        if (_hasAttackedOnce)
        {
            if (_timer >= _enemyModel.currentAttackTimeRate)
            {
                _hasAttackedOnce = false;
                _timer = 0f;
            }
            return;
        }

        // Iniciar ataque si cumplió delay inicial
        if (_timer >= _enemyModel.statsSO.AttackInitialDelay)
        {
            if (distanceToPlayer <= meleeRange)
                StartMeleeAttack();
            else
                StartCharge();
        }
    }




    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
        _rb.isKinematic = false;
    }

    public override void ResetValues()
    {
        base.ResetValues();

        _hasAttackedOnce = false;
        isCharging = false;
        chargeTimer = 0f;
        chargeDirection = Vector3.zero;

        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.ResetPath();
        }

    }


    private void EndAttackAnimations()
    {
        _enemyView.PlayAttackAnimation(false);
        _enemyView.PlayMeleeAttackAnimation(false);
    }

    private void StartMeleeAttack()
    {
        isCharging = false;
        _enemyView.PlayAttackAnimation(false);

        //enemy.SetShield(false);

        _enemyView.PlayMeleeAttackAnimation(true);
        _hasAttackedOnce = true;
        _timer = 0f;
    }
    private void StartCharge()
    {
        if (playerTransform == null) return;

        isLookingPlayer = false;

        _enemyView.PlayMeleeAttackAnimation(false);
        _enemyView.PlayAttackAnimation(true);

        enemy.SetShield(false);

        //chargeDirection = (playerTransform.position - enemy.transform.position).normalized;
        //chargeTimer = 0f;
        //isCharging = true;
        //_hasAttackedOnce = true;
        //_timer = 0f;

        //// Desactivamos NavMeshAgent para evitar conflictos
        //_navMeshAgent.enabled = false;

        //// Detener velocidad previa
        //_rb.velocity = Vector3.zero;

        // --- Nuevo: definir el punto final de charge dentro del NavMesh ---
        Vector3 rawTarget = playerTransform.position;
        Vector3 safeTarget = enemy.transform.position; // fallback

        // Buscar punto cercano al jugador dentro del NavMesh
        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            safeTarget = hit.position;
        }

        // Dirección hacia ese punto
        chargeDirection = (safeTarget - enemy.transform.position).normalized;

        // Limitar distancia máxima del charge (evita que se pase del NavMesh)
        float maxChargeDistance = 5f; // ajustá según tu diseño
        Vector3 finalTarget = enemy.transform.position + chargeDirection * maxChargeDistance;

        // Asegurar que el final del charge esté sobre el NavMesh
        if (NavMesh.SamplePosition(finalTarget, out NavMeshHit safeHit, 2f, NavMesh.AllAreas))
        {
            finalTarget = safeHit.position;
        }

        // Guardar como punto de destino final
        chargeEndPoint = finalTarget;

        // Reiniciar estados
        chargeTimer = 0f;
        isCharging = true;
        _hasAttackedOnce = true;
        _timer = 0f;

        // Desactivar NavMeshAgent para no pelear con el Rigidbody
        _navMeshAgent.enabled = false;

        // Resetear velocidad previa
        _rb.velocity = Vector3.zero;

    }

    private void EndCharge()
    {
        isCharging = false;

        // Reactivamos NavMeshAgent
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        // Detener Rigidbody
        _rb.velocity = Vector3.zero;

        _enemyView.PlayAttackAnimation(false);
        isLookingPlayer = true;

        isPostCharging = true;
        postChargeTimer = 0f;

      
    }



  

}