using UnityEngine;

public class ProjFireball : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ParticleSystem particles;

    public override void Start()
    {
        base.Start();
        particles = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public override void Move()
    {
        base.Move();
        
    }

    public override void Rotate()
    {
        base.Rotate();
        particles.transform.rotation = Quaternion.Euler(0, 0, angle-180); 
    }

    public override void OnHit()
    {
        base.OnHit();
        Object.Instantiate(Resources.Load<GameObject>("Particles/FireballAoE"), gameObject.transform.position, Quaternion.identity);
    }
}
