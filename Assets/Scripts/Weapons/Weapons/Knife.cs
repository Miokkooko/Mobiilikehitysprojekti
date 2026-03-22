using UnityEngine;

public class Knife : Weapon
{
    public Knife(GameObject owner)
      : base(owner)
    {
        projectilePrefab = Resources.Load<GameObject>("Projectiles/Knife");
    }

    public override void Fire()
    {
        base.Fire();
    }
}
