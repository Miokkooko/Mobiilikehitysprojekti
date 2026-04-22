using System.Collections.Generic;
using UnityEngine;

public class CharacterBanner : Banner
{
    public CharacterBannerItem[] CommonItems;
    public CharacterBannerItem[] RareItems;
    public CharacterBannerItem[] LegendaryItems;

    private void Start()
    {
        foreach (var item in CommonItems)
        {
            item.SetRarity();
        }
        foreach (var item in RareItems)
        {
            item.SetRarity();
        }
        foreach (var item in LegendaryItems)
        {
            item.SetRarity();
        }
    }

    public override BannerItem CommonPull()
    {
        int rand = Random.Range(0, CommonItems.Length);

        return CommonItems[rand];
    }

    public override BannerItem RarePull()
    {
        int rand = Random.Range(0, RareItems.Length);

        return RareItems[rand];
    }

    public override BannerItem LegendaryPull()
    {
        int rand = Random.Range(0, LegendaryItems.Length);

        return LegendaryItems[rand];
    }
}