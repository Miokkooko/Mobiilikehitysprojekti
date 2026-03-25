using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public class LevelUpButtonArgs
    {
        public StatusEffect effect;
        public WeaponInstance weapon;

        public LevelUpButtonArgs(StatusEffect effect, WeaponInstance weapon)
        {
            this.effect = effect;
            this.weapon = weapon;
        }
    }

    public Image UpgradeIcon;
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeRank;
    public TMP_Text UpgradeDescription;
    public TMP_Text UpgradeRankUpDescription;

    LevelUpNotification owner;
    public StatusEffect status;
    public WeaponInstance weapon;

    public event EventHandler<LevelUpButtonArgs> OnButtonPress;

    public void Initialize(LevelUpNotification parent)
    {
        owner = parent;

        /*
        UpgradeIcon.sprite = data.Icon;
        UpgradeName.text = data.Name;
        UpgradeDescription.text = data.Description;
        UpgradeRank.text = $"Rank {data.Rank}";
        UpgradeRankDescription.text = data.rankDecription;
        */
    }
    
    public void OnSelect()
    {
        Debug.Log("wow");

        OnButtonPress?.Invoke(this, new LevelUpButtonArgs(status, weapon));
    }
}
