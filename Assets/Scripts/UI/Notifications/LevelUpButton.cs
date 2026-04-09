using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public class LevelUpButtonArgs
    {
        public object upgradeData;
        public LevelUpButtonArgs(object data)
        {
            upgradeData = data;
        }
    }

    public Image UpgradeIcon;
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeRank;
    public TMP_Text UpgradeDescription;
    public TMP_Text UpgradeRankUpDescription;

    public event EventHandler<LevelUpButtonArgs> OnButtonPress;

    private object upgradeData;

    public void Initialize(string name, string description, Sprite icon, object data, string rank, string rankUpDesc)
    {
        UpgradeName.text = name;
        UpgradeDescription.text = description;
        UpgradeRank.text = rank;
        UpgradeRankUpDescription.text = rankUpDesc;

        if (icon != null)
            UpgradeIcon.sprite = icon;

        upgradeData = data;
    }

    public void OnSelect()
    {
        OnButtonPress?.Invoke(this, new LevelUpButtonArgs(upgradeData));
    }
}
