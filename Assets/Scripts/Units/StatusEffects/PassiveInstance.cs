using UnityEngine;
public class PassiveInstance
{
    public int upgradeRank = 0;

    public PassiveData data;

    StatModifierInstance instance;
    public StatModifierInstance GetInstance => instance;
    public bool CanUpgrade => upgradeRank < data.Upgrades.Length;

    public PassiveInstance(PassiveData passiveData)
    {
        data = passiveData;
        instance = new StatModifierInstance(data.BaseModifier);
    }

    public void UpgradePassive()
    {
        if (!CanUpgrade)
            return;

        instance.SetValue(instance.Value + data.Upgrades[upgradeRank]);
        upgradeRank++;
    }

    public string GetRankUpDescription()
    {
        // 1. Tunnistetaan mitä halutaan näyttää prosentteina
        bool isPercent = instance.Type == ModifierType.Percent ||
                         instance.Stat == StatType.FirerateBonus ||
                         instance.Stat == StatType.AoERadius;

        float mult = isPercent ? 100f : 1f;
        string suffix = isPercent ? "%" : "";

        // 2. Lasketaan raaka-arvot
        float curRaw = instance.Value * mult;
        float nextRaw = (instance.Value + data.Upgrades[upgradeRank]) * mult;

        // 3. Pyöristetään yhteen desimaaliin (esim. 0.2, 0.4 tai 1)
        float curRounded = Mathf.Round(curRaw * 10f) / 10f;
        float nextRounded = Mathf.Round(nextRaw * 10f) / 10f;

        return $"{instance.Stat} {curRounded}{suffix} -> {nextRounded}{suffix}";
    }

    public string GetRankUpText()
    {
        return $"{upgradeRank+1} > {upgradeRank+2}";
    }
}