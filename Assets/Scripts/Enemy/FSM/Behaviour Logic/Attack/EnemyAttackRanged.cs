using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Basic", menuName = "Enemy Logic/Attack Logic/Ranged Attack With Projectiles")]

public class EnemyAttackRanged : EnemyAttackSOBase
{
    //[SerializeField] SerializedDictionary<int, EnemyAttackRanged> m_;

    private bool _hasAttackedOnce;
    private float _strafeTimer;
    private Vector3 _strafeTarget;
    private bool _isStrafing;

    [SerializeField] private float personalDistance;
    [SerializeField] private LayerMask obstacleLayers;
    //[SerializeField] private float projectileRadius = 0.2f;
    [SerializeField] private float strafeDistance = 3f;
    [SerializeField] private float strafeCooldown = 2f;
    [SerializeField] private float strafeStopDistance = 0.2f;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _timer = 0f;
        _hasAttackedOnce = false;

        _strafeTimer = 0f;
        _isStrafing = false;

        _navMeshAgent.stoppingDistance = 0f;
        _navMeshAgent.updateRotation = true;

    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {

        base.DoFrameUpdateLogic();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        _strafeTimer += Time.deltaTime;

        if (!enemy.isWhitinCombatRadius)
        {
            enemy.fsm.ChangeState(enemy.SearchState);
            
        }
        else if (distanceToPlayer <= personalDistance)
        {
            enemy.fsm.ChangeState(enemy.EscapeState);

        }

        // Si está haciendo strafe, esperar a que termine
        if (_isStrafing)
        {
            float distance = Vector3.Distance(transform.position, _strafeTarget);
            if (distance > strafeStopDistance)
            {
                Vector3 direction = (_strafeTarget - transform.position).normalized;
                Vector3 movement = direction * _enemyModel.statsSO.moveSpeed * Time.deltaTime;

                enemy.Rb.MovePosition(enemy.Rb.position + movement);
            }
            else
            {
                EndStrafe();
            }

            return;
        }

        // Si no tiene línea de visión, intentar reposicionarse
        if (!HasLineOfSightToPlayer(out Vector3 directionToPlayer))
        {
            if (_strafeTimer >= strafeCooldown)
            {
                TryStrafe(directionToPlayer);
            }
            return;
        }

        if (!_hasAttackedOnce)
        {
            if (_timer >= _initialAttackDelay)
            {
                ShootProjectile();
                _hasAttackedOnce = true;
                _timer = 0f;
            }
        }
        else if (_timer >= _timeBetweenAttacks)
        {
            //TriggerAttackColorEffect();

            ShootProjectile();
            _timer = 0f;
        }

        
    }




    public override void Initialize(GameObject gameObject, EnemyController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();

        _enemyView.PlayAttackAnimation(false);
        //TriggerAttackColorEffect();
        _hasAttackedOnce = false;
        _isStrafing = false;

    }

    private void ShootProjectile()
    {
         _enemyView.PlayAttackAnimation(true);
    }


    private bool HasLineOfSightToPlayer(out Vector3 direction)
    {
        direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 origin = transform.position + Vector3.up * 1f; // elevar el raycast un poco

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleLayers))
        {
            Debug.DrawLine(origin, hit.point, Color.red);
            return false;
        }

        Debug.DrawLine(origin, playerTransform.position, Color.green);
        return true;
    }

    private void TryStrafe(Vector3 directionToPlayer)
    {
        Vector3 right = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3[] directions = new Vector3[]
        {
            right,
            -right
        };

        foreach (var dir in directions)
        {
            Vector3 target = transform.position + dir * strafeDistance;
            if (IsPathClear(target))
            {
                _strafeTarget = target;
                _isStrafing = true;
                _strafeTimer = 0f;
                _navMeshAgent.isStopped = true;
                _enemyView.PlayStrafeAnimation(); // Si tenés animación
                return;
            }
        }

        // Si no se puede reposicionar, cambiar a estado de escape
        enemy.fsm.ChangeState(enemy.EscapeState);
    }

    private bool IsPathClear(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        Vector3 dir = (targetPosition - origin).normalized;
        float dist = Vector3.Distance(origin, targetPosition);

        return !Physics.Raycast(origin + Vector3.up * 0.5f, dir, dist, obstacleLayers);
    }

    private void EndStrafe()
    {
        _isStrafing = false;
        _strafeTimer = 0f;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = _enemyModel.statsSO.moveSpeed;
        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;
    }
}
