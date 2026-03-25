using Unity.Mathematics;
using UnityEngine;

public class StatusEffectInstance
{
    public StatusEffect Effect { get; private set; }
    Unit Owner;
    int stacks = 1;
    float duration;

    public StatusEffectInstance(StatusEffect effect, Unit target)
    {
        Effect = effect;
        Owner = target;
        Apply();
    }

    public void HandleDuration()
    {
        if (Effect.LifetimeType == StatusLifetime.Permanent)
            return;

        duration -= Time.deltaTime;

        if (duration > 0)
            return;

        switch (Effect.StackType)
        {
            case StatusStackType.StackDecrease:
                DecrementStacks();
                break;
            default:
                Expire();
                break;
        }
       
    }

    void IncrementStacks()
    {
        stacks = math.clamp(stacks + 1, 0, Effect.MaxStacks);
    }

    void DecrementStacks()
    {
        stacks = math.clamp(stacks - 1, 0, Effect.MaxStacks);
        duration = Effect.Duration;

        Debug.Log("Decrement stacks " + stacks);
        if (stacks == 0)
            Expire();
    }

    public void Apply()
    {
        bool hasStatus = Owner.HasStatusEffect(Effect);

        if(Effect.LifetimeType != StatusLifetime.Permanent)
            duration = Effect.Duration;

        switch (Effect.StackType)
        {
            case StatusStackType.Reapply:
                if(hasStatus)
                    Effect.OnApplied();
                break;
            case StatusStackType.Stack:
                IncrementStacks();
                break;
            case StatusStackType.StackDecrease:
                stacks = Effect.MaxStacks;
                break;
            default:
                break;
        }

        if (!hasStatus)
        {
            Effect.OnApplied();
        }
    }

    public void Expire()
    {
       
        Effect.OnExpired();

        Unit.RemoveStatusEffect(Effect, Owner);
    }
}