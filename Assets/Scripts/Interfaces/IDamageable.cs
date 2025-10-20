using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damageAmount);
    void Die();
    float MaxHealth { get; }
    float CurrentHealth { get; }

    Vector3 Transform {  get; }

    //Envenenamiento Enemigo Variante verde
    void ApplyDebuffDoT(float dotDuration, float dps);

}
