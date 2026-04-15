
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
        bool isPercent = instance.Type == ModifierType.Percent;
        string currentValue = isPercent ? $"{instance.Value * 100}%" : instance.Value.ToString();
        string nextValue = isPercent ? $"{(instance.Value + data.Upgrades[upgradeRank]) * 100}%" : (instance.Value + data.Upgrades[upgradeRank]).ToString();

        return $"{instance.Stat} {currentValue} -> {nextValue}";
    }

    public string GetRankUpText()
    {
        return $"{upgradeRank+1} > {upgradeRank+2}";
    }
}