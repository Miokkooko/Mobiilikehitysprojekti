public enum StatType
{
    Damage,
    Speed,
    MaxHealth,
}

[System.Serializable]
public class StatModifier
{
    public StatType Stat;
    public float Value;
    public ModifierType Type;
    public StatusEffect source;
}

public enum ModifierType
{
    Flat,       // +5 damage
    Percent,     // +20%
    None
}