using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkerBulletModifier", menuName = "Mutations/Bullet Modifiers/Marker Bullet Effect")]
public class MarkerBulletModifierSO : BulletModifierSO
{
    [SerializeField] private float damage;
    
    [Header("Trail Settings")]
    public Material markerTrailMaterial;

    public override Material GetTrailMaterial()
    {
        return markerTrailMaterial != null ? markerTrailMaterial : base.GetTrailMaterial();
    }

    public override void OnSetup(Bullet bullet, PlayerControllerEffect player) => bullet.RegisterModifier(this, player);

    public override void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player)
    {
        var statusHandler = target.GetComponent<EnemyStatusHandler>();
        if (statusHandler != null)
        {
            string source = this.name;
            statusHandler.ApplyStatusEffect(new MarkedEffect(-1, damage, source));
            Debug.LogWarning($"[MarkerBulletModifierSO] Marker aplicado, agregado: {damage} extra");
        }
    }
}
