using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelUpArgs : NotificationBase.NotificationArgs
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
    Transform content;

    [SerializeField]
    Transform buttonPrefab;
    List<LevelUpButton> buttons = new List<LevelUpButton>();

    [SerializeField]
    CanvasGroup bgDimmerGroup;

    [SerializeField]
    CanvasGroup buttonGroup;

    float m_flTransitionTimer = 0.35f;
    float m_flDestroyBaseTime = 0.45f;

    public override void Initialize(NotificationData data)
     {
         base.Initialize(data);

         if(data is LevelUpData lud)
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

            // Set display based on type
            if (data is ModifierStatusEffect status)
            {
                lub.Initialize(status.Name, status.Description, null, data);
            }
            else if (data is WeaponData weapon)
            {
                WeaponInstance instance = LevelUpManager.Instance.GetWeaponFromPlayer(weapon);
                string prefix = instance == null ? "[NEW] " : "[LVL UP] ";
                lub.Initialize(weapon.weaponName, weapon.description, weapon.Icon, data);
            }
            else if (data is StatusEffect genericStatus)
            {
                lub.Initialize(genericStatus.Name, genericStatus.Description, null, data);
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
        // Appear animation: Move Up and remove dim
        bgDimmerGroup.alpha = 0;
        bgDimmerGroup.LeanAlpha(0.5f, m_flTransitionTimer).setIgnoreTimeScale(true);

        content.localPosition = new Vector2(0, -Screen.height);
        content.LeanMoveLocalY(0, m_flTransitionTimer).setEaseOutExpo().setIgnoreTimeScale(true).delay = 0.1f;

    }

    public void DisappearAnim()
    {
        Disappear(m_flDestroyBaseTime);
        // Disappear animation: Move down and remove dim
        content.LeanMoveLocalY(-Screen.height, m_flTransitionTimer).setEaseInExpo().setIgnoreTimeScale(true).delay = 0.05f;
        bgDimmerGroup.LeanAlpha(0f, m_flTransitionTimer).setIgnoreTimeScale(true);
    }
}
