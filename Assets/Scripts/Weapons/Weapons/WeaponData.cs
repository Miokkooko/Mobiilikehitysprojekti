
using UnityEngine;

public enum WeaponType { Axe, Bible, BloodyTears, DivineAura, Fireball, HomingArrow, Kanabo, Knives, Lightning, Mine, Scythe, Sword }
[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public GameObject projectilePrefab;

    //uusia juttuja
    public string weaponName;
    [TextArea(2,5)]
    public string description;
    public Sprite Icon;
    public WeaponType weaponType;
    public ProjectilePoolType poolType;

    [Header("Stats")]
    public float baseDamage;
    public float firerate = 1f;
    public float piercing = 1f;
    public int projectileCount = 1;
    public float projectileSpread = 22.5f;
    public float projectileSpeed = 5;
    public float projectileBurst = 1;
    public float projectileLifeTime = 2;
    public float aoeDamage = 1f;
    public float aoeRadius = 1f;
    public bool usesProjectileBurst = true;

    public StatModifier[] upgradeList;
    public StatusEffect[] effectList;
}
