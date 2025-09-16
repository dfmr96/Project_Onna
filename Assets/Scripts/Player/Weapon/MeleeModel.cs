using System;

public class MeleeModel
{
    public Action<int> OnAttackStep;
    public float Damage { get; private set; }
    public float Range { get; private set; }
    public float AttackDelay { get; private set; }
    public float TimeBetweenCombo { get; private set; }
    public float CoolDown { get; private set; }

    public MeleeModel(MeleeData data)
    {
        Damage = data.Damage;
        Range = data.Range;
        AttackDelay = data.AttackDelay;
        TimeBetweenCombo = data.TimeBetweenCombo;
        CoolDown = data.CoolDown;
    }

    public void StartAttack(int comboStep) => OnAttackStep?.Invoke(comboStep);
}
