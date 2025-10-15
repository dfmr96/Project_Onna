using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Idle-Boss Monarch Idle", menuName = "Enemy Logic/Idle Logic/Boss Monarch Still In Place")]

public class MonarchIdle : EnemyIdleSOBase
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool onlyWaiting = true;

    private float timer = 0f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        timer = 0f;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        if (timer > duration && onlyWaiting)
        {
           
            enemy.fsm.ChangeState(enemy.AttackState);

       

        }
        else
        {
            if (enemy.isWhitinCombatRadius)
            {
                enemy.fsm.ChangeState(enemy.AttackState);
            }
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

}

