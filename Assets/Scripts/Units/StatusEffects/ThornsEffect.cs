using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Thorns")]
public class ThornsEffect : StatusEffect
{
    public float ThornsPercentage;

    public override void OnTakeDamagePost(StatusEffectInstance instance, DamageContext context)
    {
        float CurrentThorns = ThornsPercentage * instance.stacks;

        DamageContext thornsContext = new DamageContext((Unit)context.Target, context.Source, CurrentThorns * context.Amount, false);

        Unit.DealDamage(thornsContext);
    }
} 
