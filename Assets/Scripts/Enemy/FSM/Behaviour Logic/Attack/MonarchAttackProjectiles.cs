using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Ranged Attack Projectiles", menuName = "Enemy Logic/Boss Attack Logic/Ranged Attack With Projectiles")]
public class MonarchAttackProjectiles : EnemyAttackSOBase
{
    private ProjectileBurstShooter _burstShooter;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

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