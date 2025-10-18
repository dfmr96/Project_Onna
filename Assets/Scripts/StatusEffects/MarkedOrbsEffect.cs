public class MarkedOrbsEffect : StatusEffect
{
    private int _orbs;
    public string Source { get; private set; }
    public int OrbsQuantityAddition => _orbs;

    public MarkedOrbsEffect(float duration, int orbsQuantityAdd, string source = null)
        : base(duration)
    {
        _orbs = orbsQuantityAdd;
        Source = source;
    }

    public override void Initialize(IDamageable damageable) => base.Initialize(damageable);

    public override bool Update(float deltaTime)
    {
        if (Duration < 0) return false;
        return base.Update(deltaTime);
    }

    public override bool IsSameType(StatusEffect other)
    {
        if (other is MarkedOrbsEffect marked)
            return marked.Source == this.Source;
        return false;
    }

    protected override void OnTick() { }
}
