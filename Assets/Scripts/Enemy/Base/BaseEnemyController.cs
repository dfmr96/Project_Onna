using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyController : MonoBehaviour
{
    public abstract float MaxHealth { get; }
    public abstract float CurrentHealth { get; }

    public abstract EnemyAttackSOBase CurrentAttackSO { get; }

    public abstract void ExecuteAttack(IDamageable target);
    //public abstract void TakeDamage(float damage);
    //public abstract void Die();
}

