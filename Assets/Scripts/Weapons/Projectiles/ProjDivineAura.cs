using UnityEngine;

public class ProjDivineAura : Projectile
{

    public override void OnEnable()
    {
        base.OnEnable();

        foreach(Enemy enemy in _enemies)
        {
            if (enemy == null)
            {
                return;
            }

            Unit.DealDamage(new DamageContext(owner, enemy, damage));
        }
        Disable();
    }

    public override void Move()
    {

    }
}
