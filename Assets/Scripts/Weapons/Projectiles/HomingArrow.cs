using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomingArrow : Projectile
{
    private Transform target;
    private List<IDamageable> alreadyHit = new List<IDamageable>();

    public override void Initialize(WeaponInstance w, Unit p, Vector3 dir)
    {
        base.Initialize(w, p, dir);
        FindNewTarget();
    } // Initialize

    public override void Update()
    {
        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Kääntää suunnan lähimpään kohteeseen
            Vector3 targetDir = (target.position - transform.position).normalized;
            direction = targetDir;
        }
        else
        {
            FindNewTarget();
        }

        base.Update(); // Kutsuu Projectilen Move() ja Rotate()
    } // Update

    private void FindNewTarget()
    {
        if (_enemies == null || _enemies.Count == 0) return;

        target = _enemies
            .Where(e => e != null && e.gameObject.activeInHierarchy)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault()?.transform;

    } // FindNewTarget

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<IDamageable>(out var d))
            {
                if (!alreadyHit.Contains(d))
                {
                    Unit.DealDamage(new DamageContext(owner, d, damage));
                    alreadyHit.Add(d);

                    OnHitParticles();

                    if (gameObject.activeInHierarchy)
                    {
                        FindNewTarget();
                    }

                } // If alreadyHit
            } // If IDamageable
        } // If Enemy
    } // OnTriggerEnter2D
} // Class HomingArrow
