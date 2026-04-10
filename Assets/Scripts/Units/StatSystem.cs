using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StatSystem
{
    private List<StatModifierInstance> modifiers = new();

    public void AddModifiers(StatModifierInstance[] mod)
    {
        foreach (StatModifierInstance modifier in mod)
        {
            AddModifier(modifier);
        }
    }
    public void AddModifier(StatModifierInstance mod)
    {
        if (modifiers.Contains(mod))
        {
            Debug.Log("ModiferStacked!");
            mod.IncrementStack();
            return;
        }
        Debug.Log("ModifierApplied!");
        modifiers.Add(mod);
    }

    public void RemoveModifiers(StatModifierInstance[] mod)
    {
        var set = new HashSet<StatModifierInstance>(mod);
        modifiers.RemoveAll(m => set.Contains(m));
    }

    public void RemoveModifier(StatModifierInstance mod)
    {
        modifiers.Remove(mod);
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }

    public float Calculate(StatType stat, float baseValue)
    {
        float flat = 0;
        float percent = 0;

        foreach (var mod in modifiers)
        {
            if (mod.GetModifier.Stat != stat) continue;

            if (mod.GetModifier.Type == ModifierType.Flat)
                flat += mod.GetModifier.Value;
            else if (mod.GetModifier.Type == ModifierType.Percent)
                percent += mod.GetModifier.Value;
        }

        return (baseValue + flat) * (1f + percent);
    }
}