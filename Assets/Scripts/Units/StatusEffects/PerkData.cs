using UnityEngine;

public enum PerkType { Damage, Firerate, Haste, HpRegen, MaxHP, Piercing, ProjectileCount, Thorns }
[CreateAssetMenu(menuName = "Perks/PerkData")]
public class PerkData : ScriptableObject
{
    public Rarity rarity;

    public string Name;
    [TextArea(2,4)]
    public string Description;

    public StatusEffect statusEffect;

    public PerkType type;
    public Sprite Icon;


    public Sprite GetIcon()
    {
        return Icon == null ? statusEffect.Icon : Icon;
    }
    public string GetName()
    {
        return string.IsNullOrEmpty(Name) ? statusEffect.Name : Name;
    }

    public string GetDescription()
    {
        return string.IsNullOrEmpty(Description) ? statusEffect.Description : Description;
    }
}