using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpArgs : NotificationArgs
{
    public object upgradeChosen;

    public LevelUpArgs(object upgrade)
    {
        upgradeChosen = upgrade;
    }
}

public class LevelUpNotification : NotificationBase
{
    [SerializeField]
    TMP_Text title;

    [SerializeField]
    TMP_Text description;

    [SerializeField]
    RectTransform content;

    [SerializeField]
    Transform buttonPrefab;
    List<LevelUpButton> buttons = new List<LevelUpButton>();

    [SerializeField]
    CanvasGroup bgDimmerGroup;

    [SerializeField]
    CanvasGroup buttonGroup;

    float m_flTransitionTimer = 0.35f;
    float m_flDestroyBaseTime = 0.45f;

    Vector2 hiddenPos;
    Vector2 shownPos;

    void Awake()
    {
        shownPos = content.localPosition;
        hiddenPos = shownPos + new Vector2(0, -Screen.height);
    }
    public override void Initialize(NotificationData data)
    {
        base.Initialize(data);

        if (data is LevelUpData lud)
        {
            SetUpgradeOptions(lud.upgradeList);
        }
    }

    public void SetUpgradeOptions(List<object> upgrades)
    {
        if (content != null && buttonPrefab != null)
        {
            foreach (var upgrade in upgrades)
            {
                SpawnButton(content, upgrade);
            }
        }
    }

    void SpawnButton(Transform parent, object data)
    {
        Transform t = Instantiate(buttonPrefab, parent);

        if (t.GetComponent<LevelUpButton>() is LevelUpButton lub)
        {
            lub.OnButtonPress += Lub_OnButtonPress;

            if (data is PassiveData passive)
            {
                PassiveInstance instance = GameManager.Instance.GetPassiveFromPlayer(passive);
                string prefix = instance != null ? "[LVL UP]" : "[NEW]";
                string rankText = "";
                string rankUpDesc = "";

                if (instance != null)
                {
                    rankText = prefix + "\n" + instance.GetRankUpText();
                    rankUpDesc = instance.GetRankUpDescription();
                }

                lub.Initialize(passive.Name, passive.Description, passive.Icon, data, rankText, rankUpDesc);
            }
            else if (data is WeaponData weapon)
            {
                WeaponInstance instance = GameManager.Instance.GetWeaponFromPlayer(weapon);
                bool isNew = instance == null;

                string prefix = instance != null ? "[LVL UP]" : "[NEW]";
                string rankText = "";
                string rankUpDesc = "";

                if (instance != null)
                {
                    rankText = prefix + "\n" + instance.GetRankUpText();
                    rankUpDesc = instance.GetRankUpDescription();
                }

                lub.Initialize(weapon.weaponName, weapon.description, weapon.Icon, data, rankText, rankUpDesc);
            }

            buttons.Add(lub);
        }
    }

    private void Lub_OnButtonPress(object sender, LevelUpButton.LevelUpButtonArgs e)
    {
        foreach (var item in buttons)
        {
            item.OnButtonPress -= Lub_OnButtonPress;
        }
        RaiseNotificationEvent(new LevelUpArgs(e.upgradeData));
        DisappearAnim();
    }

    private void OnEnable()
    {
        AppearAnim();
    }

    void AppearAnim()
    {
        bgDimmerGroup.alpha = 0;
        bgDimmerGroup.LeanAlpha(0.5f, m_flTransitionTimer).setIgnoreTimeScale(true);

        content.anchoredPosition = hiddenPos;
        content.LeanMoveY(shownPos.y, m_flTransitionTimer)
            .setEaseOutExpo()
            .setIgnoreTimeScale(true)
            .setDelay(0.1f);

    }

    public void DisappearAnim()
    {
        content.anchoredPosition = shownPos;

        bgDimmerGroup.LeanAlpha(0f, m_flTransitionTimer).setIgnoreTimeScale(true);
        content.LeanMoveY(hiddenPos.y, m_flTransitionTimer)
           .setEaseInExpo()
           .setIgnoreTimeScale(true)
           .setDelay(0.1f)
           .setOnComplete(() => Destroy(gameObject));

    }
}
