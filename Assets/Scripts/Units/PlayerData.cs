using UnityEngine;

public enum CharacterType { Knight, Noble, DarkWitch, Oni, SnowElf, Seraph, Reaper }

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/PlayerData")]
public class PlayerData : UnitData
{
    public int maxWeapons = 6;
    public int maxPassives = 6;
    public WeaponData startingWeapon;
    public Rarity rarity;
    public CharacterType characterType;

    public string GetName()
    {
        string[] name = unitName.Split(',');
        string unifiedName = name[0] + "\n" + name[1];
        return unifiedName;
    }
}