using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public class Unit : MonoBehaviour, IDamageable
{
    #region stats
    float baseDamage = 1;
    public virtual float Damage => statSystem.Calculate(StatType.Damage, baseDamage);

    float baseMaxHealth = 1;
    public virtual float MaxHealth => statSystem.Calculate(StatType.MaxHealth, baseMaxHealth);

    float health;
    public virtual float Health => Mathf.Clamp(health, 0, MaxHealth);

    float baseSpeed = 1;
    public virtual float Speed => statSystem.Calculate(StatType.Speed, baseSpeed);

    float basePiercing = 0;
    public virtual float Piercing => statSystem.Calculate(StatType.Piercing, basePiercing);

    float baseProjectileCount = 0;
    public virtual float ProjectileCount => statSystem.Calculate(StatType.ProjectileCount, baseProjectileCount);

    protected float expAmount = 1;


    Dictionary<StatusEffect, StatusEffectInstance> StatusDict;
    Dictionary<ModifierType, List<StatusEffectInstance>> statusBuckets;
    ModifierType[] ExecutionOrder = { ModifierType.Flat, ModifierType.Percent, ModifierType.None };

    public StatSystem statSystem = new StatSystem();
    #endregion

    public UnitData unitData;

    public event EventHandler<KillContext> OnKill;
    public event EventHandler<KillContext> OnDeath;

    //public Animator animator;

    void Awake()
    {
        statusBuckets = new Dictionary<ModifierType, List<StatusEffectInstance>>
        {
            { ModifierType.Flat, new List<StatusEffectInstance>() },
            { ModifierType.Percent, new List<StatusEffectInstance>() },
            { ModifierType.None, new List<StatusEffectInstance>() }
        };

        StatusDict = new Dictionary<StatusEffect, StatusEffectInstance>();

        InitializeUnit();
    }

    public void InitializeUnit()
    {
        if (unitData != null)
        {
            baseMaxHealth = unitData.maxHealth;
            baseDamage = unitData.baseDamage;
            baseSpeed = unitData.moveSpeed;
            health = baseMaxHealth;
            expAmount = unitData.xpValue;

            if(unitData.animator != null)
            {
                Animator anim = GetComponent<Animator>();
                anim.runtimeAnimatorController = unitData.animator;
            }
        }
    } // initializeUnit

    public virtual void Update()
    {
        var snapshot = StatusDict.Values.ToArray();
        foreach (var sei in snapshot)
            sei.HandleDuration();
    }

    public static void DealDamage(DamageContext context)
    {
        var statuses = context.Source.GetOrderedStatuses();

        if (context.UseStatusHooks)
            foreach (var sei in statuses)
                sei.Effect.OnDealDamagePre(sei, context);

        context.Target.TakeDamage(context);

        if (context.UseStatusHooks)
            foreach (var sei in statuses)
                sei.Effect.OnDealDamagePost(sei, context);
    }

    public virtual void TakeDamage(DamageContext context)
    {
        var victimStatuses = GetOrderedStatuses();

        if (context.UseStatusHooks)
            foreach (var sei in victimStatuses)
                sei.Effect.OnTakeDamagePre(sei, context);

        health = Mathf.Clamp(health - context.Amount, 0, MaxHealth);

        SpawnDmgPopUp(context);

        if (context.UseStatusHooks)
            foreach (var sei in victimStatuses)
                sei.Effect.OnTakeDamagePost(sei, context);

        // If we died
        if (health <= 0)
        {
            var killContext = new KillContext(context.Source, this);

            // Victim onDie effects
            foreach (var sei in victimStatuses)
                sei.Effect.OnDie(sei, killContext);

            // Killer OnKill effects
            var attackerStatuses = context.Source.GetOrderedStatuses();

            foreach (var sei in attackerStatuses)
                sei.Effect.OnKill(sei, killContext);

            // Raise events
            OnDeath?.Invoke(this, killContext);
            context.Source.OnKill?.Invoke(this, killContext);
        }
    }

    public virtual void Heal(HealContext context)
    {
        var statuses = GetOrderedStatuses();

        if(context.useHooks)
            foreach (var sei in statuses)
                sei.Effect.OnHealPre(sei, context);

        context.Target.health = Mathf.Clamp(context.Target.health + context.Amount, 0, context.Target.MaxHealth);
       
        SpawnHealthPopUp(context);

        if (context.useHooks)
            foreach (var sei in statuses)
                sei.Effect.OnHealPost(sei, context);
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

        Debug.Log($"Added {effect.Name} status to: {target.name}");
        Debug.Log("Speed -> " + target.Speed);
    }

    public static void RemoveStatusEffect(StatusEffect effect, Unit target)
    {
        if (!target.StatusDict.TryGetValue(effect, out var instance))
            return;

        target.StatusDict.Remove(effect);
        target.statusBuckets[effect.ModifierType].Remove(instance);

        Debug.Log($"Removed {effect.Name} status from: {target.name}");
        Debug.Log("Speed -> " + target.Speed);
    }

    public virtual void AddModifiers(StatModifier[] modifiers)
    {
        // idk this seems too hardcoded but i dont want to figure out a better way right now
        // If we gain maxhealth we need to save previous maxhealth incase it was a % buff since we need to heal 
        // the same amount of HP as we gained maxHP
        float prevMaxHealth = MaxHealth;

        statSystem.AddModifiers(modifiers);

        HandleMaxHealthChange(prevMaxHealth);
    }

    public virtual void HandleMaxHealthChange(float previousMaxHealth)
    {
        if (previousMaxHealth >= MaxHealth )
            return;

        float healAmount = MaxHealth - previousMaxHealth;

        Heal(new HealContext(this, healAmount, false));
    }

    public virtual void RemoveModifiersFromSource(StatusEffect source)
    {
        float prevMaxHealth = MaxHealth;
        statSystem.RemoveModifiersFromSource(source);
        HandleMaxHealthChange(prevMaxHealth);
    }
    public virtual void RemoveAllModifiers()
    {
        float prevMaxHealth = MaxHealth;
        statSystem.RemoveAllModifiers();
        HandleMaxHealthChange(prevMaxHealth);
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

    void SpawnDmgPopUp(DamageContext context)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1f;
        GameObject dmgPop = Instantiate(Resources.Load<GameObject>("Popup/DamagePopUp"), spawnPos, Quaternion.identity);
        TMP_Text tmp = dmgPop.GetComponent<TextMeshPro>();
        tmp.text = context.Amount.ToString();

        if (context.Amount > 3)
        {
            tmp.color = Color.softRed;
        }
    }

    void SpawnHealthPopUp(HealContext context)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1f;
        GameObject dmgPop = Instantiate(Resources.Load<GameObject>("Popup/DamagePopUp"), spawnPos, Quaternion.identity);
        TMP_Text tmp = dmgPop.GetComponent<TextMeshPro>();
        tmp.text = context.Amount.ToString();
        tmp.color = Color.softGreen;

    }
}