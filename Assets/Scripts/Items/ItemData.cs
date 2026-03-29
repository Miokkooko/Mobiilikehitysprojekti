using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemDrops/ItemData", order = 2)]
public class ItemData : ScriptableObject
{
    public GameObject dropPrefab;

    public int amount = 1;
    
}
