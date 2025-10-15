using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Basic", menuName = "Enemy Logic/Attack Logic/Ranged Attack With Projectiles")]

public class EnemyAttackRanged : EnemyAttackSOBase
{
     

    [Header("Ranged Settings")]
    [SerializeField] private float personalDistance;
    [SerializeField] private float rayRadius = 0.3f; // grosor del raycast
    private bool _hasAttackedOnce;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _timer = 0f;
        _hasAttackedOnce = false;

        
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

        // --- condiciones de salida ---
        if (!enemy.isWhitinCombatRadius)
        {
            ResetValues();
            enemy.fsm.ChangeState(enemy.SearchState);
            return;
        }

        if (distanceToPlayer <= personalDistance)
        {
            enemy.fsm.ChangeState(enemy.EscapeState);
            return;
        }

        if (distanceToPlayer > _enemyModel.statsSO.AttackRange + 1f)
        {
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

     

        // --- si no hay línea de visión -> intentar strafe/escape ---
        if (!HasLineOfSightToPlayer(out Vector3 directionToPlayer))
        {
            enemy.fsm.ChangeState(enemy.SearchState);

            return; // NO sigue al ataque
        }

        // --- ataque con visión clara ---
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

    }

    private void ShootProjectile()
    {
        if (!HasLineOfSightToPlayer(out _))
            return; //no hay visión -> no disparar

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
        Vector3 origin = transform.position + Vector3.up * 1f;

        if (Physics.SphereCast(origin, rayRadius, direction, out RaycastHit hit, distance))
        {
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
