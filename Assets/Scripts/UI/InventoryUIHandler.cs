using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class InventoryUIHandler : MonoBehaviour
{
    public Transform passiveContent;
    public Transform weaponContent;


    List<InventoryItemUI> passives = new List<InventoryItemUI>();
    List<InventoryItemUI> weapons = new List<InventoryItemUI>();

    public Transform ItemPrefab;

    public void Initialize(int maxWeapons, int maxPassives)
    {
        Bastor.Helpers.KillChildren(passiveContent);
        Bastor.Helpers.KillChildren(weaponContent);

        for (int i = 0; i < maxWeapons; i++) 
        {
            SpawnWeapon(weaponContent);
        }
        for (int i = 0; i < maxPassives; i++) 
        {
            SpawnPassive(passiveContent);
        }
    }

    void SpawnPassive(Transform parent)
    {
        Transform t = Instantiate(ItemPrefab, parent);

        if (t.GetComponent<InventoryItemUI>() is InventoryItemUI iui)
        {
            passives.Add(iui);
        }
    }
    void SpawnWeapon(Transform parent)
    {
        Transform t = Instantiate(ItemPrefab, parent);

        if(t.GetComponent<InventoryItemUI>() is InventoryItemUI iui)
        {
            weapons.Add(iui);
        }
    }

    public void UpdateWeaponList(WeaponData[] datas)
    {
 
        for (int i = 0; i < weapons.Count; i++) 
        {
            if (i >= datas.Length)
                return;
        
            weapons[i].Initialize(datas[i]);
        }
    }
    public void UpdatePassiveList(PassiveData[] datas)
    {
        for (int i = 0; i < passives.Count; i++) 
        {
            if (i >= datas.Length)
                return;

            passives[i].Initialize(datas[i]);
        }
    }
}
