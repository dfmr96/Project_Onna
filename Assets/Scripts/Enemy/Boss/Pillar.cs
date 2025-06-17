using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour, IDamageable
{
    public float MaxHealth => throw new System.NotImplementedException();

    public float CurrentHealth => throw new System.NotImplementedException();

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}
