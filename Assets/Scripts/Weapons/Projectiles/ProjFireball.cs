using UnityEngine;

public class ProjFireball : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ParticleSystem particles;
    Enemy target;
    Vector3 dir;

    public override void OnEnable()
    {
        base.OnEnable();
        particles = gameObject.GetComponent<ParticleSystem>();
        target = GetRandomEnemy();

        if (target == null)
        {
            Disable();
            //Destroy(gameObject);
            return;
        }

        dir = (target.transform.position - transform.position).normalized;

    }

    // Update is called once per frame
    public override void Move()
    {        
        transform.position += dir * projectileSpeed * Time.deltaTime;
    }

    public override void Rotate()
    {
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle-180);
        particles.transform.rotation = Quaternion.Euler(0, 0, angle-180); 
    }

    public override void OnHitParticles()
    {
        base.OnHitParticles();
        SpawnAoE();
    }
}
