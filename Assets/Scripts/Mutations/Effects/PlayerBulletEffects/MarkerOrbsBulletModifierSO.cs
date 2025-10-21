using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkerOrbsBulletModifier", menuName = "Mutations/Bullet Modifiers/Marker Orbs Bullet Effect")]
public class MarkerOrbsBulletModifierSO : BulletModifierSO
{
    [SerializeField] private int orbsQuantityAddition;
    
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
            statusHandler.ApplyStatusEffect(new MarkedOrbsEffect(10, orbsQuantityAddition, source));
        }
    }
}
