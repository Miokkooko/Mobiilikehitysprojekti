using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public enum Rarity { Common, Rare, Legendary }
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

    float GetLegendaryWeight(int pity)
    {
        if (pity >= legendaryMaxPity)
            return 999f; // jos et tällä rollaa legeä niin kanttee mennä lottoamaan

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

        LegendaryChanceText.SetText($"Legendary Chance: {Math.Round(LegendaryChance,2).ToString()}%");
    }
    public Rarity Pull()
    {
        currentPity++;
        pullsSinceRare++;

        CalculateChances();

        float roll = UnityEngine.Random.value;
        Rarity get;

        if (roll <= currentLegendaryChance)
        {
            get = Rarity.Legendary;
            currentPity = 0;
            Debug.Log("Legendary Get!");
            Debug.Log(DebugChances());
        }
        else if (roll <= currentRareChance + currentLegendaryChance || pullsSinceRare >= rareMaxPity)
        {
            get = Rarity.Rare;

            pullsSinceRare = 0;
            Debug.Log("Rare Get!");
            Debug.Log(DebugChances());
        }
        else
        {
            get = Rarity.Common;
            Debug.Log("Common Get!");
            Debug.Log(DebugChances());
        }
        CalculateChances();

        return get;
    }
    string DebugChances()
    {
        return $"LegendaryChance: {LegendaryChance}% | Pity: {currentPity}/{legendaryMaxPity}\nRareChance: {RareChance}% | Pity: {pullsSinceRare}/{rareMaxPity}\nCommonChance: {CommonChance}%";
    }
}

public class BannerData { }