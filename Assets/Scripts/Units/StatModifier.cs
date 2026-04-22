public enum StatType
{
    Damage,
    Speed,
    MaxHealth,
    Piercing,
    ProjectileCount,
    ProjectileBurst,
    FirerateBonus,
    AoERadius,
    XpGainPercent,
    HpRegen,
    Thorns
}

[System.Serializable]
public class StatModifier
{
    public StatType Stat;
    public float Value;
    public ModifierType Type;
}

public enum ModifierType
{
    Flat,       // +5 damage
    Percent,     // +20%
    None
}