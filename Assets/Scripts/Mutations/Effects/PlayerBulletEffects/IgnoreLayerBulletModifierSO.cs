using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IgnoreLayerBulletModifier", menuName = "Mutations/Bullet Modifiers/Ignore Layers Effect")]
public class IgnoreLayerBulletModifierSO : BulletModifierSO
{
    [Header("Trail Settings")]
    public Material traillMaterial;

    public override Material GetTrailMaterial()
    {
        return traillMaterial != null ? traillMaterial : base.GetTrailMaterial();
    }

    public override void ApplyBeforeShoot(Bullet bullet, PlayerControllerEffect player)
    {
        // registramos el modificador
        bullet.RegisterModifier(this, player);

        // activamos que ignore capas de colisión
        bullet.SetIgnoreObstacles(true);
    }

    public override void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player)
    {
        // nada extra al impactar
    }

    public void RemoveEffect(Bullet bullet)
    {
        bullet.SetIgnoreObstacles(false);
    }
}

