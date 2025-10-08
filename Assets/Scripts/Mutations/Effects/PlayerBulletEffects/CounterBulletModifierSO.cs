using UnityEngine;

[CreateAssetMenu(fileName = "CounterBulletModifier", menuName = "Mutations/Bullet Modifiers/Counter Bullet Effect")]
public class CounterBulletModifierSO : BulletModifierSO
{
    public override void OnSetup(Bullet bullet, PlayerControllerEffect player) => bullet.RegisterModifier(this, player);

    public override void OnHit(Bullet bullet, GameObject target, PlayerControllerEffect player)
    {
        if (target.TryGetComponent<IDamageable>(out _)) player?.CheckBulletHit();
        else player?.RestartBulletHit();
    }
}

