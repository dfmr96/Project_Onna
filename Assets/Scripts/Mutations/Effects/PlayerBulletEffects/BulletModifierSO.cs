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

    //se puede usar si alguna mutaci�n altera stats del arma directamente (penetraci�n, cadencia, da�o base, etc.).
    public virtual void ModifyWeapon(WeaponController weapon) { }

    // M�todo para que cada mod devuelva su material
    public virtual Material GetTrailMaterial()
    {
        return trailMaterial;
    }
}
