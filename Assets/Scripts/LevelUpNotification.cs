using NUnit.Framework;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelUpArgs : NotificationBase.NotificationArgs
{
    public int indexChosen;

    public LevelUpArgs(int index)
    {
        indexChosen = index;
        
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
            if (content != null && buttonPrefab != null)
            {
                for (int i = 0; i < lud.upgradeList.Count(); i++)
                {
                    SpawnButton(content);
                }
            }
        }   
    }

    void SpawnButton(Transform parent)
    {
        Transform t = Instantiate(buttonPrefab, parent);

        if(t.GetComponent<LevelUpButton>() is LevelUpButton lub)
        {
            lub.Initialize(this);

            buttons.Add(lub);
        }
    }

    private void OnEnable()
    {
        AppearAnim();
    }

    void AppearAnim()
    {
        // Appear animation: Move Up and remove dim
        bgDimmerGroup.alpha = 0;
        bgDimmerGroup.LeanAlpha(0.5f, m_flTransitionTimer);

        content.localPosition = new Vector2(0, -Screen.height);
        content.LeanMoveLocalY(0, m_flTransitionTimer).setEaseOutExpo().delay = 0.1f;

    }

    public void DisappearAnim()
    {
        Disappear(m_flDestroyBaseTime);
        // Disappear animation: Move down and remove dim
        content.LeanMoveLocalY(-Screen.height, m_flTransitionTimer).setEaseInExpo().delay = 0.05f;
        bgDimmerGroup.LeanAlpha(0f, m_flTransitionTimer);
    }
}
