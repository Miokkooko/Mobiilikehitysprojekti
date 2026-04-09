using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ProjLightning : Projectile
{
    Enemy target;
    
    public void OnEnable()
    {
        base.Start();
        target = GetRandomEnemy();
        if(target == null)
        {
            Disable(PoolType.Projectile_Lightning); 
            //Destroy(gameObject);
            return;
        }

        projectilePiercing = 99f;
        transform.position = target.transform.position;
    }

    public override void Move()
    {

    }
}
