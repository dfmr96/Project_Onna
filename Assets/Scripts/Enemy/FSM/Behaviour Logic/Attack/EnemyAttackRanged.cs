using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Basic", menuName = "Enemy Logic/Attack Logic/Ranged Attack With Projectiles")]

public class EnemyAttackRanged : EnemyAttackSOBase
{
        /*
        #Ataque
        7 attackStartDistance: a qué distancia empieza a atacar.
        9 attackExitDistance: si el jugador se aleja más de esto, deja de atacar.
        3.5 tooCloseDistance: si el jugador está más cerca que esto, el enemigo se pone nervioso y escapa.

        #Chase
        6 chaseMinDistance: cuando llega a esta distancia, deja de perseguir y se prepara para atacar.

        #Strafe
        3 strafeMoveDistance: cuánto se mueve lateralmente.
        0.2 strafeStopThreshold: cuando está lo suficientemente cerca del punto lateral, para.

        #Escape
        6 escapeSafeDistance: si el player está dentro de esta distancia, huye.
        8 escapeRetreatDistance: cuán lejos intenta alejarse del player.
        */

    private bool _hasAttackedOnce;
    private float _strafeTimer;
    private Vector3 _strafeTarget;
    private bool _isStrafing;

    [SerializeField] private float personalDistance;
    [SerializeField] private LayerMask obstacleLayers;
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
            EndStrafe();
            ResetValues();
            enemy.fsm.ChangeState(enemy.SearchState);
            return;
        }

        if (distanceToPlayer <= personalDistance)
        {
            EndStrafe();
            ResetValues();
            enemy.fsm.ChangeState(enemy.EscapeState);
            return;
        }

        if (_isStrafing)
        {
            HandleStrafe();
            return;
        }

        if (!HasLineOfSightToPlayer(out Vector3 directionToPlayer))
        {
            if (_strafeTimer >= strafeCooldown)
            {
                TryStrafe(directionToPlayer);
            }
            return;
        }

        _timer += Time.deltaTime;

        if (!_hasAttackedOnce && _timer >= _enemyModel.statsSO.AttackInitialDelay)
        {
            ShootProjectile();
            _hasAttackedOnce = true;
            _timer = 0f;
        }
        else if (_hasAttackedOnce && _timer >= _enemyModel.currentAttackTimeRate)
        {
            ShootProjectile();
            _timer = 0f;
        }
    }


    private void HandleStrafe()
    {
        float distance = Vector3.Distance(transform.position, _strafeTarget);
        if (distance > strafeStopDistance)
        {
            Vector3 direction = (_strafeTarget - transform.position).normalized;
            Vector3 movement = direction * _enemyModel.currentSpeed * Time.deltaTime;

            enemy.Rb.MovePosition(enemy.Rb.position + movement);
        }
        else
        {
            EndStrafe();
        }
    }



    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
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

        //ROTAR hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f; //para no inclinar hacia arriba/abajo si el jugador esta a otra altura

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); //o un valor menor para suavizar
        }
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
        _navMeshAgent.speed = _enemyModel.currentSpeed;
        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;
    }
}
