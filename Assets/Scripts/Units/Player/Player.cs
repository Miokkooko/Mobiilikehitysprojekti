using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    Dictionary<WeaponData, WeaponInstance> weapons = new Dictionary<WeaponData, WeaponInstance>();
    Dictionary<StatModifier, WeaponInstance> passives;

    PlayerMovement movement;
    // PlayerData data; // This is where we will get the BaseStats eventually


    public override void Update()
    {
        base.Update();

        FireWeapons();
        movement.MovePlayer(Speed);
    }

    void Start()
    {
        //AddWeapon(new Axe());
        movement = GetComponent<PlayerMovement>();
        OnDeath += Player_OnDeath;

        AddWeapon(Resources.Load<WeaponData>("WeaponData/KnifeData"));
    }

    private void Player_OnDeath(object sender, KillContext e)
    {
        OnDeath -= Player_OnDeath;
        Destroy(gameObject);
    }

    public virtual void FireWeapons()
    {
        foreach (KeyValuePair<WeaponData, WeaponInstance> w in weapons)
        {
            w.Value.TryFire();
        }
    }

    public void AddWeapon(WeaponInstance w)
    {
        weapons.Add(w.data, w);
    }

    public void AddWeapon(WeaponData w)
    {
        WeaponInstance instance = new WeaponInstance(this, w);
        weapons.Add(w, instance);
    }

    public void UpgradeWeapon(WeaponData weapon)
    {
        // Example:
        // var wpn = activeWeapons.Find(w => w.data == weapon);
        // wpn.LevelUp(level);

        weapons[weapon].UpgradeWeapon();
    }

    public WeaponInstance GetWeapon(WeaponData data)
    {
        try
        {
            return weapons[data];
        }
        catch (System.Exception)
        {
            return null;
        }

    }
}