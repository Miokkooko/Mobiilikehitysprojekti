using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Player : Unit
{
    List<WeaponInstance> weapons; // Holds all weapons that the player currently has

    // PlayerData data; // This is where we will get the BaseStats eventually

    void Start()
    {
        //AddWeapon(new Axe());
    }
    public virtual void FireWeapons()
    {
       foreach(WeaponInstance w in weapons)
        {
            /*
            if(w.canFire)
            {
                w.Fire();
            }
            */
        }
    }

    public void AddWeapon(WeaponInstance w)
    {
        weapons.Add(w);
    }

    
}
