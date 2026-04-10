using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;

    public int maxWeapons = 6;
    public int maxPassives = 6;

    public float maxHealth;
    public float baseDamage;
    public float moveSpeed;

    public int xpValue; // Vihuille 
                        // Kenties muitaki muuttujia niinku attack cooldown, resistances, jne
    public int coinValue;
    public int coinDropChance;

    public Sprite baseSprite;
    public AnimatorOverrideController animator;

    public WeaponData startingWeapon;

} // Class