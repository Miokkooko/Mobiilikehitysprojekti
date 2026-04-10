using System.Collections.Generic;
using UnityEngine;

public class AoE : Projectile
{
    CircleCollider2D collider2d;
    public override void Start()
    {
        Destroy(gameObject, 0.5f);
        projectilePiercing = 99f;
        collider2d = GetComponent<CircleCollider2D>();
        collider2d.radius = aoeRadius;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d)
        {
            Unit.DealDamage(new DamageContext(owner, d, aoeDamage));
        }

        if (collision.tag == "Enemy")
        {
            Enemy dummy = collision.GetComponent<Enemy>();

            if (dummy != null)
            {
                OnHitParticles();
            }
        }
    }
}
