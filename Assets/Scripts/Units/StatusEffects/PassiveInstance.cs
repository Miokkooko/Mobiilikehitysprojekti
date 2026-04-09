
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

    public string GetRankUpDescription()
    {
        bool isPercent = modifier.Type == ModifierType.Percent;
        string currentValue = isPercent ? $"{modifier.Value * 100}%" : modifier.Value.ToString();
        string nextValue = isPercent ? $"{(modifier.Value + data.Upgrades[upgradeRank]) * 100}%" : (modifier.Value + data.Upgrades[upgradeRank]).ToString();

        return $"{modifier.Stat} {currentValue} -> {modifier.Value + data.Upgrades[upgradeRank]}" + (modifier.Type == ModifierType.Percent ? "%" : "");
    }

    public string GetRankUpText()
    {
        return $"{upgradeRank+1} > {upgradeRank+2}";
    }
}