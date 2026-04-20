using UnityEngine;

public enum StatusHandling 
{ 
    Reapply,                    // Reapplies this buff (OnApplied triggers)
    Refresh,                    // Refreshes the duration of this buff (OnApplied does NOT trigger)
    StackIncrementDecrement,    // Increments Stacks | Decreases stacks by 1 when duration = 0 | Expires when stacks = 0
    StackIncrementExpire,       // Increments Stacks | Expires when duration = 0
    MaxStackDecrement,          // Instantly Max Stacks when applied / reapplied | Decreases stacks by 1 when duration = 0 | Expires when stacks = 0
}

public class StatusEffect : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    [TextArea(3, 5)]
    public string Description;
    public float Duration;
    public int MaxStacks;
    public float TickRate = 1f;

    public StatusHandling StatusHandling;
    public bool Permanent;
    public ModifierType ModifierType = ModifierType.None;
    
    public virtual void OnApplied(StatusEffectInstance instance) { }

    public virtual void OnExpired(StatusEffectInstance instance) { }

    public virtual void OnStackIncrement(StatusEffectInstance instance) { }

    public virtual void OnStackDecrement(StatusEffectInstance instance) { }

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
