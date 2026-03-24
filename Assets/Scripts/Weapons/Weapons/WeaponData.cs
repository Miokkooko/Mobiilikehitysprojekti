using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{

    public GameObject projectilePrefab;

    public float cooldown = 1f;
    public int projectileCount = 1;


}
