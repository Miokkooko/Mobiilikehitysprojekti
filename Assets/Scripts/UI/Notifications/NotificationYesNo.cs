using UnityEngine;
using TMPro;
public class NotificationYesNo : NotificationBase
{
    [SerializeField]
    TMP_Text title;

    [SerializeField]
    TMP_Text description;

    [SerializeField]
    Transform content;

    [SerializeField]
    Sprite image;

    [SerializeField]
    CanvasGroup bgDimmerGroup;

    [SerializeField]
    CanvasGroup buttonGroup;

    float m_flTransitionTimer = 0.35f;
    public override void Initialize(NotificationData data)
    {
        base.Initialize(data);

        if (title != null) 
            title.text = data.Title;

        if (description != null)
            description.text = data.Description;
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

    void DisappearAnim()
    {
        // Disappear animation: Move down and remove dim
        content.LeanMoveLocalY(-Screen.height, m_flTransitionTimer).setEaseInExpo().setOnComplete(() => Destroy(gameObject));
        bgDimmerGroup.LeanAlpha(0f, m_flTransitionTimer);
    }

    public void ConfirmButton()
    {
        RaiseNotificationEvent(new NotificationArgs(Data, true));
        buttonGroup.interactable = false;

        DisappearAnim();
        ConfirmAnimation();
    }
 

    public void CancelButton()
    {
        RaiseNotificationEvent(new NotificationArgs(Data, false));
        buttonGroup.interactable = false;
        
        DisappearAnim();
        CancelAnimation();
    }

    void ConfirmAnimation()
    {
        // Pop
        float popMaxScale = 1.2f;
        float popDuration = 0.125f;

        content.LeanScale(content.localScale * popMaxScale, popDuration).setEaseOutCubic();
        content.LeanScale(content.localScale * 1, m_flTransitionTimer).setEaseOutCubic().delay = popDuration;
    }

    void CancelAnimation()
    {
        // Shake
        float shakeDuration = 0.05f;
        float shakeMaxDistance = 50;
        content.LeanMoveLocalX(-shakeMaxDistance, shakeDuration).setEaseOutCubic();
        content.LeanMoveLocalX(shakeMaxDistance, shakeDuration).setEaseOutCubic().delay = shakeDuration;
        content.LeanMoveLocalX(-shakeMaxDistance / 2, shakeDuration).setEaseOutCubic().delay = shakeDuration * 2;
        content.LeanMoveLocalX(shakeMaxDistance / 2, shakeDuration).setEaseOutCubic().delay = shakeDuration * 3;
        content.LeanMoveLocalX(0, shakeDuration).setEaseOutCubic().delay = shakeDuration * 4;
    }

    
}
