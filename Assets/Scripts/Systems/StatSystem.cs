using System.Collections.Generic;

public class StatSystem
{
    private List<StatModifier> modifiers = new();

    public void AddModifiers(StatModifier[] mod)
    {
        modifiers.AddRange(mod);
    }

    public void RemoveModifiers(StatusEffect source)
    {
        modifiers.RemoveAll(m => m.source == source);
    }

    public float Calculate(StatType stat, float baseValue)
    {
        float flat = 0;
        float percent = 0;

        foreach (var mod in modifiers)
        {
            if (mod.Stat != stat) continue;

            if (mod.Type == ModifierType.Flat)
                flat += mod.Value;
            else if (mod.Type == ModifierType.Percent)
                percent += mod.Value;
        }

        return (baseValue + flat) * (1f + percent);
    }
}