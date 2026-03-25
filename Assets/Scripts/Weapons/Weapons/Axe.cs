using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;



public class Axe : WeaponInstance
{
    public Axe(Player owner, WeaponData data) : base(owner, data) 
    {
        //projectilePrefab = Resources.Load<GameObject>("Projectiles/Axe");
    }

    public override void Fire()
    {
        base.Fire();
    }
    
}