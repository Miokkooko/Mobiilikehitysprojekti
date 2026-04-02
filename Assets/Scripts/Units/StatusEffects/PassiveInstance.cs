using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class PassiveInstance
{
    public int upgradeRank = 0;

    public PassiveData data;

    StatModifier modifier = new StatModifier();
    public StatModifier GetModifier => modifier;
    public bool CanUpgrade => upgradeRank < data.Upgrades.Length;
    public PassiveInstance(PassiveData passiveData)
    {
        data = passiveData;
        modifier.Stat = data.BaseModifier.Stat;
        modifier.Value = data.BaseModifier.Value;
        modifier.Type = data.BaseModifier.Type;
    }

    public void UpgradePassive()
    {
        
        if (!CanUpgrade)
            return;

        modifier.Value += data.Upgrades[upgradeRank];
        upgradeRank++;
    }
}