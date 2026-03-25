using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static NotificationBase;

public class Unit : MonoBehaviour, IDamageable
{
    #region stats
    float baseDamage = 1;
    public float Damage => statSystem.Calculate(StatType.Damage, baseDamage);

    float baseMaxHealth = 1;
    public float MaxHealth => statSystem.Calculate(StatType.MaxHealth, baseMaxHealth);

    float health;
    public float Health => Mathf.Clamp(health, 0, MaxHealth);

    float baseSpeed = 1;
    public float Speed => statSystem.Calculate(StatType.Speed, baseSpeed);

    Dictionary<StatusEffect, StatusEffectInstance> StatusDict;
    Dictionary<ModifierType, List<StatusEffectInstance>> statusBuckets;
    ModifierType[] ExecutionOrder = { ModifierType.Flat, ModifierType.Percent, ModifierType.None };

    StatSystem statSystem = new StatSystem();
    #endregion

    public event EventHandler<KillContext> OnKill;
    public event EventHandler<KillContext> OnDeath;

    void Awake()
    {
        statusBuckets = new Dictionary<ModifierType, List<StatusEffectInstance>>
        {
            { ModifierType.Flat, new List<StatusEffectInstance>() },
            { ModifierType.Percent, new List<StatusEffectInstance>() },
            { ModifierType.None, new List<StatusEffectInstance>() }
        };

        StatusDict = new Dictionary<StatusEffect, StatusEffectInstance>();
    }

    public virtual void Update()
    {
        var snapshot = StatusDict.Values.ToArray();
        foreach (var sei in snapshot)
            sei.HandleDuration();
    }

    public static void DealDamage(DamageContext context)
    {
        var statuses = context.Source.GetOrderedStatuses();

        foreach (var sei in statuses)
            sei.Effect.OnDealDamagePre(context);

        context.Target.TakeDamage(context);

        foreach (var sei in statuses)
            sei.Effect.OnDealDamagePost(context);
    }

    public virtual void TakeDamage(DamageContext context)
    {
        var victimStatuses = GetOrderedStatuses();

        foreach (var sei in victimStatuses)
            sei.Effect.OnTakeDamagePre(context);

        health = Mathf.Clamp(health - context.Amount, 0, MaxHealth);

        foreach (var sei in victimStatuses)
            sei.Effect.OnTakeDamagePost(context);

        // If we died
        if (health <= 0)
        {
            var killContext = new KillContext(context.Source, this);

            // Victim onDie effects
            foreach (var sei in victimStatuses)
                sei.Effect.OnDie(killContext);

            // Killer OnKill effects
            var attackerStatuses = context.Source.GetOrderedStatuses();
            foreach (var sei in attackerStatuses)
                sei.Effect.OnKill(killContext);

            // Raise events
            OnDeath?.Invoke(this, killContext);
            context.Source.OnKill?.Invoke(this, killContext);
        }
    }

    public virtual void Heal(HealContext context)
    {
        var statuses = GetOrderedStatuses();

        foreach (var sei in statuses)
            sei.Effect.OnHealPre(context);

        context.Target.health = Mathf.Clamp(context.Target.health + context.Amount, 0, context.Target.MaxHealth);

        foreach (var sei in statuses)
            sei.Effect.OnHealPost(context);
    }

    public static void ApplyStatusEffect(StatusEffect effect, Unit target)
    {
        if (target.StatusDict.TryGetValue(effect, out var existing))
        {
            existing.Apply();
            return;
        }

        var instance = new StatusEffectInstance(effect, target);

        target.StatusDict.Add(effect, instance);
        target.statusBuckets[effect.ModifierType].Add(instance);

        // If we affect unit stats instead of Hooks, Add them to the statsystem
        if(effect is ModifierStatusEffect mse)
            target.statSystem.AddModifiers(mse.Modifiers);
        

        Debug.Log($"Added {effect.Name} status to: {target.name}");
        Debug.Log("Speed -> " + target.Speed);
    }

    public static void RemoveStatusEffect(StatusEffect effect, Unit target)
    {
        if (!target.StatusDict.TryGetValue(effect, out var instance))
            return;

        target.StatusDict.Remove(effect);
        target.statusBuckets[effect.ModifierType].Remove(instance);

        // If we affect unit stats instead of Hooks, remove all modifiers from this source
        if (effect is ModifierStatusEffect mse)
            target.statSystem.RemoveModifiers(effect);

        Debug.Log($"Removed {effect.Name} status from: {target.name}");
        Debug.Log("Speed -> " + target.Speed);
    }

    // Tähän tarvin GPT apua ja laitoin talteen koska uutta asiaa itelle
    // Returns all StatusEffectInstances in execution order as an iterable sequence.
    // IEnumerable<StatusEffectInstance> is “something you can iterate over”.
    // yield return produces each status one at a time, lazily, without creating a new list.
    // If you need a concrete list or array, you can call .ToList() or .ToArray() on the result. 
    public IEnumerable<StatusEffectInstance> GetOrderedStatuses()
    {
        foreach (var type in ExecutionOrder)
        {
            var bucket = statusBuckets[type].ToArray();
            foreach (var sei in bucket)
                yield return sei;
        }
            
    }

    public bool HasStatusEffect(StatusEffect effect)
    {
        return StatusDict.ContainsKey(effect);
    }
}
