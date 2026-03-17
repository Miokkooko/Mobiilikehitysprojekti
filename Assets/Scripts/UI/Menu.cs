using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public CanvasGroup m_canvasGroup;
    public bool m_bDisappearPreviousMenu = true;

    public void AllowInteraction(bool b)
    {
        m_canvasGroup.interactable = b;
    }

    public void Appear()
    {
        AllowInteraction(true);
        AppearAnim();
    }

    public void Disappear()
    {
        AllowInteraction(false);
        DisappearAnim();
    }

    public virtual void AppearAnim() 
    {
        gameObject.SetActive(true);
    }

    public virtual void DisappearAnim() 
    {
        gameObject.SetActive(false);
    }
}
