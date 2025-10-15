using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "SlowBulletModifier", menuName = "Mutations/Bullet Modifiers/Slow Bullet Effect")]
public class SlowBulletModifierSO : BulletModifierSO
{
    [Header("Slow Settings")]
    public float slowDuration = 3f;
    public float slowAmount = 0.5f; // 50% slow

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

    public override void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player)
    {
        var statusHandler = target.GetComponent<EnemyStatusHandler>();
        if (statusHandler != null)
        {
            string source = this.name; // usa el nombre del asset
            statusHandler.ApplyStatusEffect(new SlowEffect(slowDuration, slowAmount, source));
            Debug.Log($"[SlowBulletModifierSO] Slow aplicado {slowAmount * 100}% por {slowDuration}s");
        }
    }
}

