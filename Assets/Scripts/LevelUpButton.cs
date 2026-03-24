using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public Image UpgradeIcon;
    public TMP_Text UpgradeName;
    public TMP_Text UpgradeRank;
    public TMP_Text UpgradeDescription;
    public TMP_Text UpgradeRankUpDescription;

    LevelUpNotification owner;

    public void Initialize(LevelUpNotification parent /*WeaponData data*/)
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
        owner.RaiseNotificationEvent(new LevelUpArgs(1));
        owner.DisappearAnim();
    }
}
