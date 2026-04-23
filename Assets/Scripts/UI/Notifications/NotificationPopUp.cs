using System.Collections;
using TMPro;
using UnityEngine;
public class NotificationPopUp : NotificationBase
{
    [SerializeField]
    TMP_Text titleText;

    [SerializeField]
    TMP_Text descriptionText;

    [SerializeField]
    RectTransform content;

    [SerializeField]
    CanvasGroup buttonGroup;

    [SerializeField]
    float m_flTransitionTimer = 0.35f;

    Vector2 targetPos;
    float offscreenY;

    void Awake()
    {
        targetPos = content.anchoredPosition;

        // Move fully above: height + padding
        offscreenY = targetPos.y + content.rect.height + 200f;
    }

    public override void Initialize(NotificationData data)
    {
        base.Initialize(data);

        if (titleText != null)
            titleText.text = data.Title;

        if (descriptionText != null)
            descriptionText.text = data.Description;
    }

    public void Initialize(string title, string description)
    {
        if (titleText != null)
            titleText.SetText(title);

        if (descriptionText != null)
            descriptionText.SetText(description);
    }

    private void OnEnable()
    {
        AppearAnim();
    }

    void AppearAnim()
    {
        // Start above screen
        SetY(offscreenY);

        LeanTween.value(gameObject, offscreenY, targetPos.y, m_flTransitionTimer)
            .setEaseOutExpo()
            .setDelay(0.1f)
            .setIgnoreTimeScale(true)
            .setOnUpdate(SetY)
            .setOnComplete(() =>
                StartCoroutine(DisappearAfterDelay(2f)));
    }

    void DisappearAnim()
    {
        buttonGroup.interactable = false;

        LeanTween.value(gameObject, targetPos.y, offscreenY, m_flTransitionTimer)
            .setEaseInExpo()
            .setIgnoreTimeScale(true)
            .setOnUpdate(SetY)
            .setOnComplete(() => Destroy(gameObject));
    }

    void SetY(float y)
    {
        var pos = content.anchoredPosition;
        pos.y = y;
        content.anchoredPosition = pos;
    }

    public void OnClick()
    {
        StopAllCoroutines();
        DisappearAnim();
    }

    IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DisappearAnim();
    }
}
