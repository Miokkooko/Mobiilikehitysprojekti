using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Bleed")]
public class BleedEffect : StatusEffect
{
    public float tickDamage = 1f;

    public override void OnTick(StatusEffectInstance instance)
    {
        Debug.Log("Bleed Tick");

        Unit target = instance.Owner;

        float totalDamage = tickDamage * instance.stacks;

        DamageContext context = new DamageContext(
            target,      
            target,      
            totalDamage,  
            false        
        );

        Unit.DealDamage(context);

    } // OnTick
} // class BleedEffect
