using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend-Tank use Shield for a time", menuName = "Enemy Logic/Defend Logic/Tank use Shield for a time")]

public class EnemyDefendTank : EnemyDefendSOBase
{
    private float defendDuration;
    private float timer;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        enemyModel = gameObject.GetComponent<EnemyModel>();

        //enemyModel.SetShield(true);
        defendDuration = Random.Range(minDefendTime, maxDefendTime);
        timer = 0f;

    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        if (timer >= defendDuration)
        {
            enemy.fsm.ChangeState(enemy.AttackState);
        }
    }

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        //enemyModel.SetShield(false);

    }

}
