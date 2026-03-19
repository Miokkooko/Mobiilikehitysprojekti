using UnityEngine;

public enum StatusStackType { Reapply, Refresh, Stack, StackDecrease }
public enum StatusLifetime { Duration, Stacks, Permanent }

public class StatusEffect : ScriptableObject
{
    public string Name;
    [TextArea(3, 5)]
    public string Description;
    public float Duration;
    public int MaxStacks;

    public StatusStackType StackType;
    public StatusLifetime LifetimeType;
    public ModifierType ModifierType = ModifierType.None;
    
    public virtual void OnApplied() { }

    public virtual void OnExpired() { }

    public virtual void OnDealDamagePre(DamageContext context) { }

    public virtual void OnDealDamagePost(DamageContext context) { }

    public virtual void OnTakeDamagePre(DamageContext context) { }

    public virtual void OnTakeDamagePost(DamageContext context) { }

    public virtual void OnHealPre(HealContext context) { }

    public virtual void OnHealPost(HealContext context) { }

    public virtual void OnKill(KillContext context) { }

    public virtual void OnDie(KillContext context) { }

    public virtual void OnTick() { }

}
