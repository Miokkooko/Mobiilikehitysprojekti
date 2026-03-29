using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;

    public float maxHealth;
    public float baseDamage;
    public float moveSpeed;

    public int xpValue; // Vihuille 
                        // Kenties muitaki muuttujia niinku attack cooldown, resistances, jne

} // Class
