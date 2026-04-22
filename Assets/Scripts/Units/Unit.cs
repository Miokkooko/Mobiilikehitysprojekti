using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


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
    public virtual float Speed => Mathf.Clamp(statSystem.Calculate(StatType.Speed, baseSpeed), 0.25f, 100);

    float basePiercing = 0;
    public virtual float Piercing => statSystem.Calculate(StatType.Piercing, basePiercing);

    float baseProjectileCount = 0;
    public virtual float ProjectileCount => statSystem.Calculate(StatType.ProjectileCount, baseProjectileCount);

    float baseProjectileBurst = 0;
    public virtual float ProjectileBurst => statSystem.Calculate(StatType.ProjectileBurst, baseProjectileBurst);

    float baseFireratePercent = 1;
    public virtual float FireratePercent => statSystem.Calculate(StatType.FirerateBonus, baseFireratePercent);
  
    float baseBonusXpGain = 1;
    public virtual float XpGainPercent => statSystem.Calculate(StatType.XpGainPercent, baseBonusXpGain);

    float baseAoERadius = 1;
    public virtual float AoERadius => statSystem.Calculate(StatType.AoERadius, baseAoERadius);


    Dictionary<StatusEffect, StatusEffectInstance> StatusDict;
    Dictionary<ModifierType, List<StatusEffectInstance>> statusBuckets;
    ModifierType[] ExecutionOrder = { ModifierType.Flat, ModifierType.Percent, ModifierType.None };

    public StatSystem statSystem = new StatSystem();
    #endregion

    public event EventHandler<KillContext> OnKill;
    public event EventHandler<KillContext> OnDeath;

    protected bool isKnockedBack;
    private bool canDamageOnCollision = false;

    [SerializeField] protected bool canBeKnockedBack = true;

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
    }


    public virtual void InitializeUnit(UnitData data)
    {
        if (data != null)
        {
            baseMaxHealth = data.maxHealth;
            baseDamage = data.baseDamage;
            baseSpeed = data.moveSpeed;
            health = baseMaxHealth;

            if (data.animator != null)
            {
                Animator anim = GetComponent<Animator>();
                anim.runtimeAnimatorController = data.animator;
            }

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (data.baseSprite != null)
            {
                sr.sprite = data.baseSprite;
            }
        }

    } // initializeUnit

    public virtual void Update()
    {
        if (Health <= 0)
            return;

        var snapshot = StatusDict.Values.ToArray();
        foreach (var sei in snapshot)
        {
            sei.HandleDuration();
        }
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

        SpawnDmgPopUp(context);
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration, bool canDamageOthers = false)
    {
        if (!canBeKnockedBack)
            return;

        if (gameObject.activeInHierarchy)
        {

            // jos voi vahingoittaa törmäyksessä voi vahingoittaa muita
            StopAllCoroutines();
            this.canDamageOnCollision = canDamageOthers;
            StartCoroutine(KnockbackRoutine(direction, force, duration));

        }
        
    } // ApplyKnockback

    IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {

        if (!gameObject.activeInHierarchy)
            yield break;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            isKnockedBack = true;

            rb.linearVelocity = direction.normalized * force;

            yield return new WaitForSeconds(duration);

            rb.linearVelocity = Vector2.zero;
            isKnockedBack = false;
            canDamageOnCollision = false;
        }
    } // IEnumerator KnockbackRoutine


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKnockedBack && canDamageOnCollision)
        {
            // Tarkistaa osuiko knockbackatty toiseen viholliseen
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                float impactForce = GetComponent<Rigidbody2D>().linearVelocity.magnitude;

                canDamageOnCollision = false;

                DamageContext context = new DamageContext(this, damageable, impactForce * 1.5f, false);
                damageable.TakeDamage(context);


                // ANNETAAN knockback kohteelle (vihollinen B)
                if (damageable is Unit unitTarget)
                {
                    Vector2 pushDir = (collision.transform.position - transform.position).normalized;

                    // jotta sen oma coroutine nollaa nopeuden 0.15 sekunnin päästä
                    unitTarget.ApplyKnockback(pushDir, impactForce * 0.7f, 0.15f, false);

                    // Valinnainen: Hidasta myös hyökkääjää (A) osuman jälkeen
                    GetComponent<Rigidbody2D>().linearVelocity *= 0.5f;
                }
            }
        }
    } // Void OnCollisionEnter2D



    public virtual void TakeDamage(DamageContext context)
    {
        var victimStatuses = GetOrderedStatuses();

        if (context.UseStatusHooks)
            foreach (var sei in victimStatuses)
                sei.Effect.OnTakeDamagePre(sei, context);

        health = Mathf.Clamp(health - context.Amount, 0, MaxHealth);

        

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

            return;
        }

        if (context.UseStatusHooks && gameObject.activeInHierarchy)
        {
            var targetStatuses = GetOrderedStatuses();
            foreach (var sei in targetStatuses)
            {
                // Jos jokin aiempi efekti tässä loopissa otti vihollisen pois käytöstä
                if (!gameObject.activeInHierarchy) break;

                sei.Effect.OnTakeDamagePost(sei, context);
            }
        }

    }

    public virtual void Heal(HealContext context, bool showPopUp = true)
    {
        var statuses = GetOrderedStatuses();

        if (context.useHooks)
            foreach (var sei in statuses)
                sei.Effect.OnHealPre(sei, context);

        context.Target.health = Mathf.Clamp(context.Target.health + context.Amount, 0, context.Target.MaxHealth);

        if(showPopUp)
            SpawnHealthPopUp(context);

        if (context.useHooks)
            foreach (var sei in statuses)
                sei.Effect.OnHealPost(sei, context);
    }

    public static void ApplyStatusEffect(StatusEffect effect, Unit target)
    {
        if (target.StatusDict.TryGetValue(effect, out var existing))
        {
            float prevMaxHP = target.MaxHealth;
            existing.Apply();
            target.HandleMaxHealthChange(prevMaxHP);
            return;
        }

        var instance = new StatusEffectInstance(effect, target);

        target.StatusDict.Add(effect, instance);
        target.statusBuckets[effect.ModifierType].Add(instance);

        Debug.Log($"Added {effect.Name} status to: {target.name}");
    }

    public static void RemoveStatusEffect(StatusEffect effect, Unit target)
    {
        if (!target.StatusDict.TryGetValue(effect, out var instance))
            return;

        target.StatusDict.Remove(effect);
        target.statusBuckets[effect.ModifierType].Remove(instance);

        Debug.Log($"Removed {effect.Name} status from: {target.name}");
    }

    public static void RemoveAllStatusEffects(Unit target)
    {
        target.StatusDict.Clear(); ;
        foreach (var effect in target.statusBuckets)
        {
            effect.Value.Clear();
        }
    }

    public virtual void HandleMaxHealthChange(float previousMaxHealth)
    {
        if (previousMaxHealth >= MaxHealth)
            return;

        float healAmount = MaxHealth - previousMaxHealth;

        Heal(new HealContext(this, healAmount, false));
    }

    public virtual void AddModifiers(StatModifierInstance[] modifiers)
    {
        float prevMaxHealth = MaxHealth;
        statSystem.AddModifiers(modifiers);
        HandleMaxHealthChange(prevMaxHealth);
    }
    public virtual void AddModifier(StatModifierInstance modifier)
    {
        float prevMaxHealth = MaxHealth;
        statSystem.AddModifier(modifier);

        if(modifier.Stat == StatType.MaxHealth)
            HandleMaxHealthChange(prevMaxHealth);
    }

    public virtual void RemoveModifiers(StatModifierInstance[] modifiers)
    {
        float prevMaxHealth = MaxHealth;
        statSystem.RemoveModifiers(modifiers);
        HandleMaxHealthChange(prevMaxHealth);
    }
    public virtual void RemoveModifier(StatModifierInstance modifier)
    {
        float prevMaxHealth = MaxHealth;
        statSystem.RemoveModifier(modifier);
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

    static void SpawnDmgPopUp(DamageContext context)
    {
        Vector3 spawnPos = ((MonoBehaviour)context.Target).transform.position + Vector3.up * 1f;
        GameObject dmgPop = PoolManager.Instance.SpawnPopUp(spawnPos);
        TMP_Text tmp = dmgPop.GetComponent<TextMeshPro>();
        tmp.text = context.Amount.ToString();

        if (context.Amount > 3)
        {
            tmp.color = Color.softRed;
        }
    }

    static void SpawnHealthPopUp(HealContext context)
    {
        Vector3 spawnPos = (context.Target).transform.position + Vector3.up * 1f;
        GameObject dmgPop = PoolManager.Instance.SpawnPopUp(spawnPos);


        TMP_Text tmp = dmgPop.GetComponent<TextMeshPro>();
        tmp.text = context.Amount.ToString();
        tmp.color = Color.softGreen;

    }
}