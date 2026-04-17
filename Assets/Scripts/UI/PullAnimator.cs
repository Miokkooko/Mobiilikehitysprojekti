using System;
using UnityEngine;
using UnityEngine.UI;

public class PullAnimator : MonoBehaviour
{
    public Action OnAnimationFinished;

    public Animator animator;
    public Image starImage;

    public Color commonColor;
    public Color rareColor;
    public Color legendaryColor;

    public void StartAnimation(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                starImage.color = commonColor;
                break;
            case Rarity.Rare:
                starImage.color = rareColor;
                break;
            case Rarity.Legendary:
                starImage.color = legendaryColor;
                break;
            default:
                starImage.color = commonColor;
                break;
        }

        gameObject.SetActive(true);
        animator.Play("PullAnimation");
    }

    public void FinishAnimation()
    {
        gameObject.SetActive(false);
        OnAnimationFinished?.Invoke();
    }
}
