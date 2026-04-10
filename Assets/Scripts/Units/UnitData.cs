using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;

    public float maxHealth;
    public float baseDamage;
    public float moveSpeed;

    public Sprite baseSprite;
    public AnimatorOverrideController animator;
} 
