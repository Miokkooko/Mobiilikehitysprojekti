using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : Unit
{
    Dictionary<WeaponData, WeaponInstance> Weapons = new Dictionary<WeaponData, WeaponInstance>();
    public List<WeaponData> GetWeapons => Weapons.Keys.ToList();
    Dictionary<PassiveData, PassiveInstance> Passives = new Dictionary<PassiveData, PassiveInstance>();
    public List<PassiveData> GetPassives => Passives.Keys.ToList();

    public List<StatusEffect> OnHitEffects = new List<StatusEffect>();

    public virtual float HpRegen => statSystem.Calculate(StatType.HpRegen, 0); // Oletus 0

    PlayerMovement Movement;
    public PlayerData playerData;
    //leveling
    protected float totalExp;
    protected float expMultiplier = 0f;
    protected float baseExpRequirement = 10f;
    protected int level = 1;

    float previousXPReq = 0;

    public float RequiredExp => baseExpRequirement + expMultiplier;
    public float PreviousRequiredExp => previousXPReq;
    public float CurrentExp => totalExp;
    public int CurrentLevel => level;

    public bool CanGetWeapon => Weapons.Count < playerData.maxWeapons;
    public bool CanGetPassive => Passives.Count < playerData.maxPassives;

    public event Action<float> OnPlayerHealthChanged;
    public event Action<float> OnPlayerExpChanged;
    public event Action<int> OnPlayerLevelUp;

    public event Action<PassiveData[]> OnPlayerGetPassive;
    public event Action<WeaponData[]> OnPlayerGetWeapon;

    private float regenTimer;

    public override void Update()
    {
        base.Update();
        FireWeapons();
        Movement.MovePlayer(Speed);

        if (HpRegen > 0 && Health < MaxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f)
            {
                Heal(new HealContext(this, HpRegen, true), false);
                regenTimer = 0;
            }
        }
    }

    public override void InitializeUnit(UnitData data)
    {
        base.InitializeUnit(data);
        playerData = (PlayerData)data;
        OnPlayerHealthChanged?.Invoke(Health);
    }

    void Start()
    {
        Movement = GetComponent<PlayerMovement>();
        OnDeath += Player_OnDeath;

        InitializeUnit(DataManager.Instance.CharacterData);
        Debug.Log("Starting weapon: " + playerData.startingWeapon);
        AddWeapon(playerData.startingWeapon);
        AddPerks();
    }
    void AddPerks()
    {
        foreach (var item in DataManager.Instance.SelectedPerks)
        {
            int x = RankManager.GetRank(SaveManager.GetPerkEntry(item.type).value);
            for (int i = 0; i < x; i++)
            {
                ApplyStatusEffect(item.statusEffect, this);
            }
        }
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
        AddModifier(Passives[data].GetInstance);

        OnPlayerGetPassive?.Invoke(Passives.Keys.ToArray());
        Debug.Log("Player got " + data.Name);
    }

  
    public void UpgradePassive(PassiveData data)
    {
        float prevMaxHp = MaxHealth;
        Passives[data].UpgradePassive();

        if (Passives[data].GetInstance.Stat == StatType.MaxHealth)
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
        if (w == null)
        {
            Debug.LogError("Where weapon bro");
            return;
        }
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
        gameObject.SetActive(false);
    }

    public override void TakeDamage(DamageContext context)
    {
        base.TakeDamage(context);

        OnPlayerHealthChanged?.Invoke(Health);
    }
    public override void Heal(HealContext context, bool showPopUp = true)
    {
        base.Heal(context, showPopUp);

        OnPlayerHealthChanged?.Invoke(Health);
    }

    public override void HandleMaxHealthChange(float previousMaxHealth)
    {
        if (previousMaxHealth >= MaxHealth)
            return; 

        base.HandleMaxHealthChange(previousMaxHealth);
        OnPlayerHealthChanged?.Invoke(Health);
    }
    #endregion

    #region LevelingSystem
    public void IncreaseExp(float amount)
    {
        amount = amount * XpGainPercent;

        totalExp += amount;

        OnPlayerExpChanged?.Invoke(totalExp);

        while (totalExp >= RequiredExp)
        {
            previousXPReq = RequiredExp;
            expMultiplier += 10*level;
            level += 1;
            OnPlayerLevelUp?.Invoke(level);
        }
    }
    #endregion
}