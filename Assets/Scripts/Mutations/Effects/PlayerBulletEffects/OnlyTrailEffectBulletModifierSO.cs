using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "OnlyTrailEffectBulletModifier", menuName = "Mutations/Bullet Modifiers/Trail Bullet Effect")]

public class OnlyTrailEffectBulletModifierSO : BulletModifierSO
{
    [Header("Trail Settings")]
    public Material slowTrailMaterial;

    public override Material GetTrailMaterial()
    {
        return slowTrailMaterial != null ? slowTrailMaterial : base.GetTrailMaterial();
    }

    public override void OnSetup(Bullet bullet, PlayerControllerEffect player)
    {
        bullet.RegisterModifier(this, player);
    }
}
