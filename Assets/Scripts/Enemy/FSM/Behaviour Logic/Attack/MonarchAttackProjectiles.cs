using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Projectiles", menuName = "Enemy Logic/Boss Attack Logic/Ranged Attack With Projectiles")]
public class MonarchAttackProjectiles : EnemyAttackSOBase
{
    private ProjectileBurstShooter _burstShooter;
    [SerializeField] private float messageDuration = 4f;
    [SerializeField] List<string> bossMessage;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        int randomIndex = Random.Range(0, bossMessage.Count);
        _bossModel.PrintMessage(bossMessage[randomIndex], messageDuration);

        _navMeshAgent.stoppingDistance = 0f;
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