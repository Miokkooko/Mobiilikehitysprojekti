using UnityEngine;

public class ProjMine : Projectile
{
    public void OnEnable()
    {
        base.Start();
        if(player != null)
            transform.position = player.transform.position;
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
