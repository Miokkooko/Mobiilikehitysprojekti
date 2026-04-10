using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/PlayerData")]
public class PlayerData : UnitData
{
    public int maxWeapons = 6;
    public int maxPassives = 6;
    public WeaponData startingWeapon;
}