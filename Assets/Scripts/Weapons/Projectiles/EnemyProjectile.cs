using UnityEngine;

public class EnemyProjectile : Projectile
{
    public virtual void EnemyInitialize(WeaponData w, Enemy e, Vector3 dir)
    {
        damage = w.baseDamage;
        projectilePiercing = w.piercing;
        projectileSpeed = w.projectileSpeed;
        direction = dir.normalized;
        enemy = e;
        projectileLifetime = w.projectileLifeTime;
        aoeDamage = w.aoeDamage;
        aoeRadius =w.aoeRadius;

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
            return;



        if (collision.GetComponent<IDamageable>() is IDamageable d && collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            Unit.DealDamage(new DamageContext(enemy, player, damage));
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
}
