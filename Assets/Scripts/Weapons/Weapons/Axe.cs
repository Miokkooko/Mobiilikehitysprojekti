using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;



public class Axe : Weapon
{
    public Axe(GameObject owner)
        : base(owner) 
    {
        projectilePrefab = Resources.Load<GameObject>("Projectiles/Axe");
    }

    public override void Fire()
    {
        base.Fire();
    }
}