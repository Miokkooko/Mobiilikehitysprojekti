using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/KnockBack")]
public class KnockBack : StatusEffect
{

    [Header("Knockback Settings")]
    public float force = 5f;          // Kuinka kovaa vihollinen lentää taaksepäin
    public float upwardsForce = 0f;   // Jos haluat nostetta ilmaan
    public bool affectMultipleTargets = false; // Törmääkö muuhun viholliseen
    public float duration = 0.2f;     // Kuinka kauan knockback kestää

    public override void OnTakeDamagePost(StatusEffectInstance sei, DamageContext context)
    {
        // Tarkistetaan, onko kohde olemassa ja aktiivinen (ei poolissa)
        if (sei.Owner != null && sei.Owner.gameObject.activeInHierarchy)
        {
            // Lasketaan suunta osuman kautta
            Vector2 dir = context.HitDirection;

            // FAILSAFE: Jos ollaan päällekkäin (pituus on lähes nolla)
            if (dir.sqrMagnitude < 0.0001f)
            {
                dir = (sei.Owner.transform.position - context.Source.transform.position).normalized;
            }

            // Käynnistetään knockback kohteen omassa skriptissä
            sei.Owner.ApplyKnockback(dir, force, duration, affectMultipleTargets);
        }
    } // OnTakeDamagePost

} // Class KnockBack
