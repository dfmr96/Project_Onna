using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Attack-Torret Ranged Attack", menuName = "Enemy Logic/Attack Logic/Torret Ranged Attack With Projectiles")]

public class EnemyAttackTorrent : EnemyAttackSOBase
{

    private bool _hasAttackedOnce;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _navMeshAgent.updateRotation = true;
        _timer = 0f;
        _hasAttackedOnce = false;


    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();


        if (!enemy.isWhitinCombatRadius)
        {
            ResetValues();
            enemy.fsm.ChangeState(enemy.IdleState);
            return;
        }

       

        _timer += Time.deltaTime;

        if (!_hasAttackedOnce && _timer >= _initialAttackDelay)
        {
            ShootProjectile();
            _hasAttackedOnce = true;
            _timer = 0f;
        }
        else if (_hasAttackedOnce && _timer >= _timeBetweenAttacks)
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

        

    }


    private void ShootProjectile()
    {
        //ROTAR hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f; //para no inclinar hacia arriba/abajo si el jugador esta a otra altura

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); //o un valor menor para suavizar
        }

        _enemyView.TorretShootProjectileFunc();
    }


   

}
