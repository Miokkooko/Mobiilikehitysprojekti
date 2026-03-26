using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{

    public GameObject projectilePrefab;

    public float cooldown = 1f;
    public int projectileCount = 1;

    //uusia juttuja
    public string weaponName;
    public string description;
    public Sprite icon;
    public int maxLevel = 8;

    [Header("Stats")]
    public float baseDamage;
    public float damagePerLevel = 10f;
}
