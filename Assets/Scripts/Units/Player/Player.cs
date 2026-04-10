using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Unit
{
    Dictionary<WeaponData, WeaponInstance> Weapons = new Dictionary<WeaponData, WeaponInstance>();
    Dictionary<PassiveData, PassiveInstance> Passives = new Dictionary<PassiveData, PassiveInstance>();

    public List<StatusEffect> OnHitEffects = new List<StatusEffect>();

    PlayerMovement Movement;
    public PlayerData playerData;
    //leveling
    protected float totalExp;
    protected float expMultiplier = 0f;
    protected float baseExpRequirement = 10f;
    protected int level = 1;

    public float PreviousRequiredExp = 0;

    public float RequiredExp => baseExpRequirement + expMultiplier;
    public float CurrentExp => totalExp;
    public int CurrentLevel => level;

    public bool CanGetWeapon => Weapons.Count < playerData.maxWeapons;
    public bool CanGetPassive => Passives.Count < playerData.maxPassives;

    public event Action<float> OnPlayerHealthChanged;
    public event Action<float> OnPlayerExpChanged;
    public event Action<int> OnPlayerLevelUp;

    public event Action<PassiveData[]> OnPlayerGetPassive;
    public event Action<WeaponData[]> OnPlayerGetWeapon;

    public override void Update()
    {
        base.Update();

        FireWeapons();
        Movement.MovePlayer(Speed);
    }
    public override void InitializeUnit(UnitData data)
    {
        base.InitializeUnit(data);
        playerData = (PlayerData)data;
    }

    void Start()
    {
        //AddWeapon(new Axe());
        

        Movement = GetComponent<PlayerMovement>();
        OnDeath += Player_OnDeath;

        InitializeUnit(playerData);
        AddWeapon(Resources.Load<WeaponData>("WeaponData/KnifeData"));
    }
    #region Passives

    public void AddPassive(PassiveData data)
    {
        if (Passives.Count >= playerData.maxPassives)
        {
            Debug.Log("Player doesn't have any passive slots left!");
            return;
        }
        Passives.Add(data, new PassiveInstance(data));
        AddModifiers(new StatModifier[] { Passives[data].GetModifier });

        OnPlayerGetPassive?.Invoke(Passives.Keys.ToArray());
        Debug.Log("Player got " + data.Name);
    }
    public void UpgradePassive(PassiveData data)
    {
        float prevMaxHp = MaxHealth;
        Passives[data].UpgradePassive();
        HandleMaxHealthChange(prevMaxHp);
        
        Debug.Log("Player upgraded " + data.Name);
    }

    public PassiveInstance GetPassive(PassiveData data)
    {
        try
        {
            return Passives[data];
        }
        catch (Exception)
        {
            return null;
        }
    }

    #endregion

    #region Weapons
    public virtual void FireWeapons()
    {
        foreach (KeyValuePair<WeaponData, WeaponInstance> w in Weapons)
        {
            w.Value.TryFire();
        }
    }

    public void AddWeapon(WeaponInstance w)
    {
        AddWeapon(w.data);
    }

    public void AddWeapon(WeaponData w)
    {
        if(Weapons.Count >= playerData.maxWeapons)
        {
            Debug.Log("Player doesn't have any weapon slots left!");
            return;
        }
        WeaponInstance instance = new WeaponInstance(this, w);
        Weapons.Add(w, instance);
        OnPlayerGetWeapon?.Invoke(Weapons.Keys.ToArray());
        Debug.Log("Player got " + w.weaponName);
    }

    public void UpgradeWeapon(WeaponData weapon)
    {
        // Example:
        // var wpn = activeWeapons.Find(w => w.data == weapon);
        // wpn.LevelUp(level);

        Weapons[weapon].UpgradeWeapon();
        Debug.Log("Player upgraded " + weapon.weaponName);
    }

    public WeaponInstance GetWeapon(WeaponData data)
    {
        try
        {
            return Weapons[data];
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    #endregion

    #region Events
    private void Player_OnDeath(object sender, KillContext e)
    {
        OnDeath -= Player_OnDeath;
        Destroy(gameObject);
    }

    public override void TakeDamage(DamageContext context)
    {
        base.TakeDamage(context);

        OnPlayerHealthChanged?.Invoke(Health);
    }
    public override void Heal(HealContext context)
    {
        base.Heal(context);

        OnPlayerHealthChanged?.Invoke(Health);
    }

    public override void HandleMaxHealthChange(float previousMaxHealth)
    {
        Debug.Log("PreviousHP = " + previousMaxHealth + " | CurrentHP = " + MaxHealth);
        if (previousMaxHealth >= MaxHealth)
            return; 

        base.HandleMaxHealthChange(previousMaxHealth);
        OnPlayerHealthChanged?.Invoke(Health);
    }
    #endregion

    #region LevelingSystem
    public void IncreaseExp(float amount)
    {
        totalExp += amount;

        OnPlayerExpChanged?.Invoke(totalExp);

        if (totalExp >= RequiredExp)
        {
            PreviousRequiredExp = RequiredExp;
            expMultiplier += 10*level;
            level += 1;
            OnPlayerLevelUp?.Invoke(level);
        }
    }
    #endregion
}