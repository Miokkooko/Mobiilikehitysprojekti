using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    float Duration;
    Unit Owner;

    public virtual void OnApplied() { }

    public virtual void OnExpired() { }

    public virtual void Apply(Unit target)
    {
        Owner = target;
        OnApplied();
    }

    public virtual void Expire()
    {
        OnExpired();
    }

    public virtual void Tick()
    {

    }

    public IEnumerator HandleLifeTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        Expire();
    }           
}
