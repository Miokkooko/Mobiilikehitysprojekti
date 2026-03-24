using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NotificationData", menuName = "Data/NotificationData", order = 2)]
public class NotificationData : ScriptableObject
{
    public int id;
    public string Title = "Attention!";

    [TextArea(7, 10)]
    public string Description = "This is a notification";

    public NotificationType Type;

    //public string[] m_sAdditionalText;
}


public class LevelUpData : NotificationData
{
    public List<StatusEffect> upgradeList;
    //public Weapon[] weaponList;

}