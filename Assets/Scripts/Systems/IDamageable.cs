using NUnit.Framework;
using System;

public class DamageContext
{
    public Unit Source { get; }
    public IDamageable Target { get; }
    public float Amount { get; set; }

    /// <summary>
    /// Wether or not this damage instance should proc status effects.
    /// Disable this when statuses deal damage do units.
    /// </summary>
    public bool UseStatusHooks { get; set; }

    public DamageContext(Unit source, IDamageable target, float amount, bool useStatusHooks = true)
    {
        Source = source;
        Target = target;
        Amount = amount;
        UseStatusHooks = useStatusHooks;
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