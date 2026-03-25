using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    List<WeaponInstance> weapons = new List<WeaponInstance>();

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

        AddWeapon(Resources.Load<WeaponData>("WeaponData/AxeData"));
    }

    private void Player_OnDeath(object sender, KillContext e)
    {
        OnDeath -= Player_OnDeath;
        Destroy(gameObject);
    }

    public virtual void FireWeapons()
    {
        foreach (WeaponInstance w in weapons)
        {
            w.TryFire();
        }
    }

    public void AddWeapon(WeaponInstance w)
    {
        weapons.Add(w);
    }

    public void AddWeapon(WeaponData w)
    {
        WeaponInstance instance = new WeaponInstance(this, w);
        weapons.Add(instance);
    }
}
