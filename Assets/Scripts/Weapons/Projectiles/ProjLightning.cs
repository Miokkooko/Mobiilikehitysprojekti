using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ProjLightning : Projectile
{
    Enemy target;
    
    public override void Start()
    {
        base.Start();
        target = GetRandomEnemy();
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        projectilePiercing = 99f;
        transform.position = target.transform.position;
    }

    public override void Move()
    {

    }
}
