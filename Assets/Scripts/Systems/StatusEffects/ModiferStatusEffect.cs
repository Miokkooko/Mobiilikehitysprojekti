using UnityEngine;

[CreateAssetMenu(fileName = "StatModiferStatus", menuName = "StatusEffects/StatModiferStatus", order = 2)]
public class ModifierStatusEffect : StatusEffect
{
    public StatModifier[] Modifiers;
}
