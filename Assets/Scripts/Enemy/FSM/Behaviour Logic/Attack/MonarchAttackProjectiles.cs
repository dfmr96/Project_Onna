using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Projectiles", menuName = "Enemy Logic/Boss Attack Logic/Ranged Attack With Projectiles")]
public class MonarchAttackProjectiles : EnemyAttackSOBase
{

 
    [Header("Boss Messages")]
    [SerializeField] private float messageDuration = 4f;
    [SerializeField] List<string> bossMessage;

    private bool doOnce = true;
    private ProjectileBurstShooter _burstShooter;



    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        if(doOnce)
        {
            int randomIndex = Random.Range(0, bossMessage.Count);
            _bossModel.PrintMessage(bossMessage[randomIndex], messageDuration);
            doOnce = false;
        }
      

        //_navMeshAgent.stoppingDistance = 0f;
        _navMeshAgent.updateRotation = true;

        _burstShooter = _bossModel.GetComponentInChildren<ProjectileBurstShooter>();

        if (_burstShooter != null)
        {
            _burstShooter.StartBurstLoop();
        }
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _burstShooter?.StopBurstLoop();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        if (!enemy.isWhitinCombatRadius)
        {
            enemy.fsm.ChangeState(enemy.IdleState);
        }
    }
}