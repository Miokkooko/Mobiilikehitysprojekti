using System;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public class LevelUpButtonArgs
    {
        //uutta
        public object upgradeData;
        public LevelUpButtonArgs(object data) { upgradeData = data; }
        //vanhaa
        /*public StatusEffect effect;
        public WeaponInstance weapon;

        public LevelUpButtonArgs(StatusEffect effect, WeaponInstance weapon)
        {
            this.effect = effect;
            this.weapon = weapon;
        }*/
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

    //uutta
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Image iconImage;

    private object upgradeData;

    public void Setup(string name, string description, Sprite icon, object data)
    {
        nameText.text = name;
        descText.text = description;
        if (icon != null) iconImage.sprite = icon;
        upgradeData = data;

        button.onClick.AddListener(() => OnButtonPress?.Invoke(this, new LevelUpButtonArgs(upgradeData)));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}


/* vanhaa
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
    /*}
    
    public void OnSelect()
    {
        Debug.Log("wow");

        OnButtonPress?.Invoke(this, new LevelUpButtonArgs(status, weapon));
    }
}*/
