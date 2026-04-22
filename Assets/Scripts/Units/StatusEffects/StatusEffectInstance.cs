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

        if (Effect.Permanent)
            return;

        duration -= Time.deltaTime;

        if (duration > 0)
            return;

        switch (Effect.StatusHandling)
        {
            case StatusHandling.MaxStackDecrement:
                DecrementStacks();
                break;
            case StatusHandling.StackIncrementDecrement:
                DecrementStacks();
                break;
            default:
                Expire();
                break;
        }
       
    }

    void IncrementStacks()
    {
        stacks++;
        stacks = math.clamp(stacks, 0, Effect.MaxStacks);
        duration = Effect.Duration;
        Debug.Log("Stacks incremented");
        Effect.OnStackIncrement(this);
    }

    void DecrementStacks()
    {
        stacks--;
        stacks = math.clamp(stacks, 0, Effect.MaxStacks);
        duration = Effect.Duration;

        if (stacks == 0)
            Expire();
        else
            Effect.OnStackDecrement(this);
    }

    public void Apply()
    {
        bool hasStatus = Owner.HasStatusEffect(Effect);

        if(!Effect.Permanent)
            duration = Effect.Duration;

        if (!hasStatus)
        {
            Effect.OnApplied(this);
            return;
        }

        switch (Effect.StatusHandling)
        {
            case StatusHandling.Reapply:
                if(hasStatus)
                    Effect.OnApplied(this);
                break;
            case StatusHandling.StackIncrementDecrement:
                IncrementStacks();
                break;
            case StatusHandling.StackIncrementExpire:
                IncrementStacks();
                break;
            case StatusHandling.MaxStackDecrement:
                stacks = Effect.MaxStacks;
                break;
            default:
                break;
        }
    }

    public void Expire()
    {
        Effect.OnExpired(this);
        Unit.RemoveStatusEffect(Effect, Owner);
    }
}