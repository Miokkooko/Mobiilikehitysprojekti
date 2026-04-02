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
        SpawnAoE();
    }
}
