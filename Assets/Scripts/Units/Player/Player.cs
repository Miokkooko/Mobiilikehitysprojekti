using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    Dictionary<WeaponData, WeaponInstance> Weapons = new Dictionary<WeaponData, WeaponInstance>();
    Dictionary<PassiveData, PassiveInstance> Passives = new Dictionary<PassiveData, PassiveInstance>();

    PlayerMovement Movement;

    //leveling
    protected float totalExp;
    protected float expMultiplier = 0f;
    protected float baseExpRequirement = 10f;
    protected int level = 1;

    public float PreviousRequiredExp = 0;

    public float RequiredExp => baseExpRequirement + expMultiplier;
    public float CurrentExp => totalExp;
    public int CurrentLevel => level;

    public event Action<float> OnPlayerHealthChanged;
    public event Action<float> OnPlayerExpChanged;
    public event Action<int> OnPlayerLevelUp;

    public override void Update()
    {
        base.Update();

        FireWeapons();
        Movement.MovePlayer(Speed);
    }

    void Start()
    {
        //AddWeapon(new Axe());
        Movement = GetComponent<PlayerMovement>();
        OnDeath += Player_OnDeath;

        AddWeapon(Resources.Load<WeaponData>("WeaponData/KnifeData"));
    }
    #region Passives

    public void AddPassive(PassiveData data)
    {
        Passives.Add(data, new PassiveInstance(data));
        statSystem.AddModifier(Passives[data].GetModifier);
    }
    public void UpgradePassive(PassiveData data)
    {
        Passives[data].UpgradePassive();
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
        Weapons.Add(w.data, w);
    }

    public void AddWeapon(WeaponData w)
    {
        WeaponInstance instance = new WeaponInstance(this, w);
        Weapons.Add(w, instance);
    }

    public void UpgradeWeapon(WeaponData weapon)
    {
        // Example:
        // var wpn = activeWeapons.Find(w => w.data == weapon);
        // wpn.LevelUp(level);

        Weapons[weapon].UpgradeWeapon();
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
    #endregion

    #region LevelingSystem
    public void IncreaseExp(float amount)
    {
        totalExp += amount;

        OnPlayerExpChanged?.Invoke(totalExp);

        if (totalExp >= RequiredExp)
        {
            LevelUpManager.Instance.TriggerLevelUp();
            PreviousRequiredExp = RequiredExp;
            expMultiplier += 10*level;
            level += 1;
            OnPlayerLevelUp?.Invoke(level);
        }
    }
    #endregion
}