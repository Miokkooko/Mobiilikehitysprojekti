using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image icon;

    public void Initialize(PassiveData data)
    {
        icon.sprite = data.Icon;
        icon.color = Color.white;
    }
    public void Initialize(WeaponData data)
    {
        icon.sprite = data.Icon;
        icon.color = Color.white;
    }
}
