using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public enum Rarity { Common, Rare, Legendary }


[Serializable]
public class BannerItem 
{
    [HideInInspector]
    public Rarity rarity; 
}

[Serializable]
public class CharacterBannerItem : BannerItem
{
    public PlayerData charData;
    public void SetRarity()
    {
        rarity = charData.rarity;
    }
}

[Serializable]
public class PerkBannerItem : BannerItem
{
    public PerkData upgradeData;
    public void SetRarity()
    {
        rarity = upgradeData.rarity;
    }
}

public class Banner : MonoBehaviour
{
    public TMP_Text LegendaryChanceText;

    int currentPity = 0;
    float commonBaseChance = 1f;
    float currentCommonChance;

    [Header("Rare")]
    int pullsSinceRare = 0;
    public int rareMaxPity = 5;
    float baseRareChance = 0.15f;
    float currentRareChance;
    
    [Header("Legendary")]
    public int legendarySoftPity = 10;
    public int legendaryMaxPity = 15;
    float currentLegendaryChance;
    float baseLegendaryChance = 0.025f;
    float chancePerPity = 0.005f;
    float chancePerSoftPity => 1f / (legendaryMaxPity - legendarySoftPity);

    public float LegendaryChance => currentLegendaryChance * 100;
    public float RareChance => currentRareChance * 100;
    public float CommonChance => currentCommonChance * 100;

    public Action<BannerItem> CommonPulled;
    public Action<BannerItem> RarePulled;
    public Action<BannerItem> LegendaryPulled;

    float GetLegendaryWeight(int pity)
    {
        if (pity >= legendaryMaxPity)
            return 99999f; // jos et tällä rollaa legeä niin kanttee mennä lottoamaan

        float w = baseLegendaryChance;

        for (int i = 0; i < pity; i++)
        {
            if (i < legendarySoftPity)
                w += chancePerPity;
            else
                w += chancePerSoftPity;
        }

        return w;
    }

    void CalculateChances()
    {
        float legendaryWeight = GetLegendaryWeight(currentPity);
        float rareWeight = baseRareChance;
        float commonWeight = commonBaseChance;

        float total = legendaryWeight + rareWeight + commonWeight;

        currentLegendaryChance = legendaryWeight / total;
        currentRareChance = rareWeight / total;
        currentCommonChance = commonWeight / total;

        //LegendaryChanceText.SetText($"Legendary Chance: {Math.Round(LegendaryChance,2).ToString()}%");
    }

    public BannerItem Pull()
    {
        currentPity++;
        pullsSinceRare++;
        CalculateChances();

        float roll = UnityEngine.Random.value;
        BannerItem item = null;

        if (roll <= currentLegendaryChance)
        {
            currentPity = 0;
            item = LegendaryPull();
        }
        else if (roll <= (currentRareChance + currentLegendaryChance) || pullsSinceRare >= rareMaxPity)
        {
            pullsSinceRare = 0;
            item = RarePull();
        }
        else
            item = CommonPull();

        return item;
    }

    public virtual BannerItem LegendaryPull() { return null; }
    public virtual BannerItem RarePull() { return null; }
    public virtual BannerItem CommonPull() { return null; }

    string DebugChances()
    {
        return $"LegendaryChance: {LegendaryChance}% | Pity: {currentPity}/{legendaryMaxPity}\nRareChance: {RareChance}% | Pity: {pullsSinceRare}/{rareMaxPity}\nCommonChance: {CommonChance}%";
    }
}