using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurnBulletModifier", menuName = "Mutations/Bullet Modifiers/Burn")]

public class BurnBulletModifierSO : BulletModifierSO
{
    [Header("Quemadura Settings")]
    public float burnDurationMin = 3f;
    public float burnDurationMax = 3f;
    public float damagePerTick = 2f;
    public float bonusDamageIfAlreadyBurned = 10f;
    public bool IsMajor = true;

    // Se llama cuando la bala se instancia
    public override void OnSetup(Bullet bullet, PlayerControllerEffect player)
    {
        // La bala no necesita añadirlo, solo se asegura que lo conozca
        bullet.RegisterModifier(this, player);
    }

    // Se llama cuando la bala impacta
    public override void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player)
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            var statusHandler = target.GetComponent<EnemyStatusHandler>();
            if (statusHandler != null)
            {
                string source = this.name; // usa el nombre del asset como fuente
                //Debug.Log("NOMBRE DE SOURCE: " + source);
                bool alreadyBurned = statusHandler.HasStatusEffect<BurnEffect>(source);

                statusHandler.ApplyStatusEffect(new BurnEffect(Random.Range(burnDurationMin, burnDurationMax), damagePerTick, source));

                if (alreadyBurned && IsMajor)
                    damageable.TakeDamage(bonusDamageIfAlreadyBurned);
            }
        }
    }
}

