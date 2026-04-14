
public class StatModifierInstance
{
    int stacks = 1;
    int maxStacks = 1;

    public StatModifier data;
    public float Value => modifier.Value;
    public ModifierType Type => modifier.Type;
    public StatType Stat => modifier.Stat;


    StatModifier modifier = new StatModifier();
    public StatModifier GetModifier => modifier;
    public StatModifier GetBaseModifierData => data;

    public StatModifierInstance(StatModifier baseModifier, int stackLimit = 1)
    {
        data = baseModifier;
        SetStats();
        maxStacks = stackLimit;
    }

    public void IncrementStack()
    {
        if (stacks >= maxStacks)
            return;

        stacks++;

        modifier.Value = data.Value * stacks;
    }

    public void DecrementStack()
    {
        if (stacks <= 0)
            return;

        stacks--;

        modifier.Value = data.Value * stacks;
    }

    void SetStats()
    {
        modifier.Stat = data.Stat;
        modifier.Value = data.Value * stacks;
        modifier.Type = data.Type;
    }

    public void SetValue(float value)
    {
        modifier.Value = value;
    }
}