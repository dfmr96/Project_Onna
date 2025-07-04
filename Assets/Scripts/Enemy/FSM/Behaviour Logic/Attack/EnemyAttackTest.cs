using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Attack-test", menuName = "Enemy Logic/Attack Logic/test")]
public class EnemyAttackTest : EnemyAttackSOBase
{


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        Debug.Log("Entro en attack test");
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        Debug.Log("Salio attack test");

        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        Debug.Log("Updating attack test");



    }


    public override void ResetValues()
    {
        base.ResetValues();
        
    }

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }
}
