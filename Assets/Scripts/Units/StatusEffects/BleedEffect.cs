using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Bleed")]
public class BleedEffect : StatusEffect
{
    public float tickDamage = 1f;

    public override void OnTick(StatusEffectInstance instance)
    {
       
        Unit target = instance.Owner;

        float totalDamage = tickDamage * instance.stacks;
        Debug.Log("Bleed Tick: " + tickDamage + " * " + instance.stacks + " = " + totalDamage);

        DamageContext context = new DamageContext(
            target,      
            target,      
            totalDamage,  
            false        
        );

        Unit.DealDamage(context);

    } // OnTick
} // class BleedEffect
