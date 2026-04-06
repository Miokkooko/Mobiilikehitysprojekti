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
    public float TickRate = 1f;

    public StatusStackType StackType;
    public StatusLifetime LifetimeType;
    public ModifierType ModifierType = ModifierType.None;
    
    public virtual void OnApplied(StatusEffectInstance instance) { }

    public virtual void OnExpired(StatusEffectInstance instance) { }

    public virtual void OnDealDamagePre(StatusEffectInstance instance, DamageContext context) { }

    public virtual void OnDealDamagePost(StatusEffectInstance instance, DamageContext context) { }

    public virtual void OnTakeDamagePre(StatusEffectInstance instance, DamageContext context) { }

    public virtual void OnTakeDamagePost(StatusEffectInstance instance, DamageContext context) { }

    public virtual void OnHealPre(StatusEffectInstance instance, HealContext context) { }

    public virtual void OnHealPost(StatusEffectInstance instance, HealContext context) { }

    public virtual void OnKill(StatusEffectInstance instance, KillContext context) { }

    public virtual void OnDie(StatusEffectInstance instance, KillContext context) { }

    public virtual void OnTick(StatusEffectInstance instance) { }

}
