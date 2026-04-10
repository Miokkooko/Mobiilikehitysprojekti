using UnityEngine;

[CreateAssetMenu(fileName = "StatModiferStatus", menuName = "StatusEffects/StatModiferStatus", order = 2)]
public class ModifierStatusEffect : StatusEffect
{
    public StatModifier[] Modifiers;

    StatModifierInstance[] instances;

    public override void OnApplied(StatusEffectInstance instance)
    {
        instances = new StatModifierInstance[Modifiers.Length];

        for (int i = 0; i < Modifiers.Length; i++) 
        {
            instances[i] = new StatModifierInstance(Modifiers[i], MaxStacks);
            Debug.Log(instances[i].GetModifier.ToString());
        }
        
        instance.Owner.AddModifiers(instances);
        Debug.Log("OnApplied " + instance.Owner.Speed);
    }

    public override void OnStackIncrement(StatusEffectInstance instance)
    {
        foreach (var item in instances)
        {
            item.IncrementStack();
        }

        Debug.Log("OnIncrement " + instance.Owner.Speed);
    }
    public override void OnStackDecrement(StatusEffectInstance instance)
    {
        foreach (var item in instances)
        {
            item.DecrementStack();
        }
        Debug.Log("OnDecrement " + instance.Owner.Speed);
    }

    public override void OnExpired(StatusEffectInstance instance)
    {
        instance.Owner.RemoveModifiers(instances);
        Debug.Log("OnExpired " + instance.Owner.Speed);
    }
}
