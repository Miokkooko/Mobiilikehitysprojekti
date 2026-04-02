using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/BurnEffect")]
public class BurnEffect : StatusEffect
{
    public float tickDamage = 3f;

    public override void OnTick(StatusEffectInstance instance)
    {
        Unit target = instance.Owner;

        DamageContext context = new DamageContext(
            target,
            target,
            tickDamage,
            false
        );

        Unit.DealDamage(context);

    } // OnTick
} // class BurnEffect
    

