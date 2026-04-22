
using UnityEngine;

public class AoE : Projectile
{
    public override void OnEnable()
    {
        StopAllCoroutines();
        AoEFallBack = ProjectilePoolType.Projectile_AoE;
        StartCoroutine(DestroyAfterdelay(0.5f));
        projectilePiercing = 99f;
        transform.localScale = new Vector2(aoeRadius, aoeRadius);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            return;

        if (collision.GetComponent<IDamageable>() is IDamageable d)
        {
            Unit.DealDamage(new DamageContext(owner, d, damage));
        }

        if (collision.tag == "Enemy")
        {
            Enemy dummy = collision.GetComponent<Enemy>();

            if (dummy != null)
            {
                OnHit();
            }
        }
    }

    public override void Move()
    {
        
    }
}
