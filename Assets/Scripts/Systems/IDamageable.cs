using NUnit.Framework;
using System;

public class DamageContext
{
    public Unit Source { get; }
    public IDamageable Target { get; }
    public float Amount { get; set; }

    public DamageContext(Unit source, IDamageable target, float amount)
    {
        Source = source;
        Target = target;
        Amount = amount;
    }
}
public class HealContext
{
    public Unit Target { get; }
    public float Amount { get; set; }

    public HealContext(Unit target, float amount)
    {
        Target = target;
        Amount = amount;
    }
}
public class KillContext
{
    public Unit Target { get; }
    public Unit Source { get; }

    public KillContext(Unit source, Unit target)
    {
        Target = target;
        Source = source;
    }
}

public interface IDamageable
{
    void TakeDamage(DamageContext context);
    void Heal(HealContext context);
}