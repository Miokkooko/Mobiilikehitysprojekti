using UnityEngine;

public class EnemyProjectile : Projectile
{
    ParticleSystem particles;
    private void Start()
    {
        particles = gameObject.GetComponent<ParticleSystem>();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d && collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            Unit.DealDamage(new DamageContext(owner, player, damage));
        }

        if (collision.tag == "Player")
        {
            Enemy dummy = collision.GetComponent<Enemy>();

            if (dummy != null)
            {
                OnHitParticles();
            }
        }
    }

    public override void Rotate()
    {
        base.Rotate();
        particles.transform.rotation = Quaternion.Euler(0, 0, angle - 180);
    }
}
