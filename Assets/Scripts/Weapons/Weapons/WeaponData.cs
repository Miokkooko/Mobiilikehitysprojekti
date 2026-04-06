using System;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public GameObject projectilePrefab;

    //uusia juttuja
    public string weaponName;
    public string description;
    public Sprite Icon;

    [Header("Stats")]
    public float baseDamage;
    public float firerate = 1f;
    public float piercing = 1f;
    public int projectileCount = 1;
    public float projectileSpeed = 5;
    public float projectileLifeTime = 2;
    public float aoeDamage = 1f;
    public float aoeRadius = 1f;

    public WeaponModifier[] upgradeList;
    public StatusEffect[] effectList;
}
