using System.Collections;
using System.Collections.Generic;
using Player;
using Player.Stats.Runtime;
using UnityEngine;

public class HubEndTrigger : LevelTrigger
{
    protected override void OnTrigger(Collider other)
    {
        PlayerHelper.DisableInput(); 
        LoadNextLevel(); 
        RunData.Initialize();
    }
}
