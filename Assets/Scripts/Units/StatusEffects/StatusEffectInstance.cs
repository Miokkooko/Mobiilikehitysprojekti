using Unity.Mathematics;
using UnityEngine;

public class StatusEffectInstance
{
    public StatusEffect Effect { get; private set; }
    public Unit Owner { get; private set; }
    public int stacks = 1;
    public float duration;

    float lastTick;
    float tickRate = 1f;

    public StatusEffectInstance(StatusEffect effect, Unit target)
    {
        Effect = effect;
        Owner = target;
        tickRate = effect.TickRate;
        Apply();
    }

    void TryTick()
    {
        if (Time.time > lastTick + tickRate)
        {
            lastTick = Time.time;
            Effect.OnTick(this);
        }
    }

    public void HandleDuration()
    {
        TryTick();   

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
                    Effect.OnApplied(this);
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
            Effect.OnApplied(this);
        }
    }

    public void Expire()
    {
       
        Effect.OnExpired(this);

        Unit.RemoveStatusEffect(Effect, Owner);
    }
}