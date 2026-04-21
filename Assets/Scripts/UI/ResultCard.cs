using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultCard : MonoBehaviour
{
    public Image resultIcon;
    public Image bg;

    public Color commonColor;
    public Color rareColor;
    public Color legendaryColor;

    public RankBar rankBar;

    public CanvasGroup alphaGroup;
    public float transitionTime = 1;
    int currentCopies = 0;

    public Action<ResultCard> OnDisappear;

    public TMP_Text nameText;
    public TMP_Text rankLevelText;
    public TMP_Text rankText;

    public void Initialize(Rarity rarity, Sprite icon, int currentOwned, string name)
    {
        switch (rarity)
        {
            case Rarity.Common:
                bg.color = commonColor;
                break;
            case Rarity.Rare:
                bg.color = rareColor;
                break;
            case Rarity.Legendary:
                bg.color = legendaryColor;
                break;
        }
        currentCopies = currentOwned;

        Debug.Log(currentCopies + " | " + RankManager.GetRankProgress(currentCopies));
        rankBar.Initialize(0, RankManager.GetRankProgress(currentCopies), RankManager.CopiesPerRank, currentCopies);

        resultIcon.sprite = icon;
        nameText.SetText(name);

        rankBar.xpBar.BarFilled += HandleRankUp;

    }
    private void OnDestroy()
    {
        OnDisappear?.Invoke(this);
    }

    private void OnDisable()
    {
        if (rankBar.xpBar != null)
            rankBar.xpBar.BarFilled -= HandleRankUp;
    }

    void HandleRankUp()
    {
        if (this == null || gameObject == null || gameObject.Equals(null))
            return;

        rankBar.RankUp();
    }

    void HandleRankLevel()
    {
        if (currentCopies >= RankManager.MaxCopies)
            return;

        rankBar.SetCurrentCopies(currentCopies + 1);
    }

    public void AppearAnim()
    {
        gameObject.SetActive(true);

        // Appear animation: Move Up and remove dim
        alphaGroup.alpha = 0;
        alphaGroup.LeanAlpha(1, transitionTime).setOnComplete(() => alphaGroup.blocksRaycasts = true);

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - 375);
        transform.LeanMoveLocalY(0, transitionTime).setEaseOutExpo().setOnComplete(() => HandleRankLevel());
    }

    public void DisappearAnim()
    {
        LeanTween.cancel(gameObject);
        alphaGroup.blocksRaycasts = false;
        alphaGroup.alpha = 1;
        alphaGroup.LeanAlpha(0, transitionTime / 2);

        transform.LeanMoveLocalY(-375, transitionTime / 2).setEaseInExpo().setOnComplete(() => OnDisappear?.Invoke(this));
    }
}
