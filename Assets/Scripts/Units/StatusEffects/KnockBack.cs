using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/KnockBack")]
public class KnockBack : StatusEffect
{

    [Header("Knockback Settings")]
    public float force = 5f;          // Kuinka kovaa vihollinen lentää taaksepäin
    public float upwardsForce = 0f;   // Jos haluat nostetta ilmaan
    public bool affectMultipleTargets = false; // Törmääkö muuhun viholliseen
    public float duration = 0.2f;     // Kuinka kauan knockback kestää


} // Class KnockBack
