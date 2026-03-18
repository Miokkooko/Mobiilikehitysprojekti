public enum StatType
{
    Damage,
    Speed,
}

public class StatModifier
{
    public StatType Stat;
    public float Value;
    public ModifierType Type;
}

public enum ModifierType
{
    Flat,       // +5 damage
    Percent     // +20%
}