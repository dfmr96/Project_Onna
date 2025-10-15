using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileCollectable
{
    void OnHitByProjectile(PlayerControllerEffect shooterOwner);
}

