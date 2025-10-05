using Player.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletModifierSO : ScriptableObject
{
    public Material trailMaterial;

    //se llama al instanciar la bala
    public virtual void OnSetup(Bullet bullet, PlayerControllerEffect player) { }

    //se llama al impactar
    public virtual void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player) { }

    //se puede usar si alguna mutación altera stats del arma directamente (penetración, cadencia, daño base, etc.).
    public virtual void ModifyWeapon(WeaponController weapon) { }

    // Método para que cada mod devuelva su material
    public virtual Material GetTrailMaterial()
    {
        return trailMaterial;
    }

    // Se llama antes de disparar (Setup inicial de la bala)
    public virtual void ApplyBeforeShoot(Bullet bullet, PlayerControllerEffect player)
    {
        // Por defecto hace lo mismo que OnSetup
        OnSetup(bullet, player);
    }
}
