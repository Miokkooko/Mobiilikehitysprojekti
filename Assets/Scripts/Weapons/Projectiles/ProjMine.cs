using UnityEngine;

public class ProjMine : Projectile
{
    public override void Start()
    {
        base.Start();
        transform.position = player.transform.position;
    }
    public override void Move()
    {

    }

    public override void OnHit()
    {
        base.OnHit();
        Object.Instantiate(Resources.Load<GameObject>("Particles/FireballAoE"), gameObject.transform.position, Quaternion.identity);
    }
}
