using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "StatModiferStatus", menuName = "StatusEffects/StatModiferStatus", order = 2)]
public class ModifierStatusEffect : StatusEffect
{
    public StatModifier[] Modifiers;

    public override void OnApplied(StatusEffectInstance instance)
    {
        instance.Owner.statSystem.AddModifiers(Modifiers);
    }

    public override void OnExpired(StatusEffectInstance instance)
    {
        instance.Owner.statSystem.RemoveModifiersFromSource(this);
    }
}
