using UnityEngine;

public class ProjMine : Projectile
{
    public override void OnEnable()
    {
        base.OnEnable();
        if(owner != null)
            transform.position = owner.transform.position;
    }
    public override void Move()
    {

    }

    public override void OnHitParticles()
    {
        base.OnHitParticles();
        SpawnAoE();
    }
}
