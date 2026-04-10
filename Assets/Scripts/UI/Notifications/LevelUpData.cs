using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelUpData", menuName = "Data/LevelUpData", order = 2)]
public class LevelUpData : NotificationData
{
    public List<object> upgradeList;
}