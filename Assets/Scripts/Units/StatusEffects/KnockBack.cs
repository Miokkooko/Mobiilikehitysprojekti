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
            // Lasketaan suunta
            Vector2 dir = (sei.Owner.transform.position - context.Source.transform.position).normalized;

            // Käynnistetään knockback kohteen omassa skriptissä
            sei.Owner.ApplyKnockback(dir, force, duration);
        }
    } // OnTakeDamagePost

} // Class KnockBack
