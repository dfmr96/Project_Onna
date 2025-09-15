public class MeleeModel
{
    public float Damage { get; private set; }
    public float Range { get; private set; }
    public float AttackDelay { get; private set; }
    public float CoolDown { get; private set; }

    public MeleeModel(MeleeData data)
    {
        Damage = data.Damage;
        Range = data.Range;
        AttackDelay = data.AttackDelay;
        CoolDown = data.CoolDown;
    }
}
