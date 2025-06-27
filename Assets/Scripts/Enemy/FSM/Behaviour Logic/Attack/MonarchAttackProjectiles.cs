using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Projectiles", menuName = "Enemy Logic/Boss Attack Logic/Ranged Attack With Projectiles")]

public class MonarchAttackProjectiles : EnemyAttackSOBase
{
    //[SerializeField] SerializedDictionary<int, EnemyAttackRanged> m_;

    private bool _hasAttackedOnce;


    [SerializeField] private LayerMask obstacleLayers;
    //[SerializeField] private float projectileRadius = 0.2f;
  


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

        if (!enemy.isWhitinCombatRadius)
        {
            enemy.fsm.ChangeState(enemy.IdleState);
            
        }
    


        // Si no tiene línea de visión, intentar reposicionarse
        //if (!HasLineOfSightToPlayer(out Vector3 directionToPlayer))
        //{

        //    enemy.fsm.ChangeState(enemy.IdleState);
        //    return;
        //}

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




    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();

        //_bossView.PlayAttackAnimation(false);
        //TriggerAttackColorEffect();
        _hasAttackedOnce = false;

    }

    private void ShootProjectile()
    {
        Debug.Log("disparo");
        //_bossView.PlayAttackAnimation(true);
        _bossView.PlayProjectilesAttackAnimation();

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

    

    private bool IsPathClear(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        Vector3 dir = (targetPosition - origin).normalized;
        float dist = Vector3.Distance(origin, targetPosition);

        return !Physics.Raycast(origin + Vector3.up * 0.5f, dir, dist, obstacleLayers);
    }

   
}
