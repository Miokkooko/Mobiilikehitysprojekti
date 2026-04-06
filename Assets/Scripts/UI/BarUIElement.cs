using UnityEngine;
using UnityEngine.UI;

public enum UIBarType { Fade, Lerp, }
public class BarUIElement : MonoBehaviour
{
    public SlicedFilledImage Bar;
    public SlicedFilledImage UnderBar;

    public float MaxValue;
    public float MinValue = 0;
    public float CurrentValue;

    public UIBarType BarType;

    public void SetMaxValue(float value) 
    {
        MaxValue = value;
        CalculateBarFillAmount();
    }

    public void SetMinValue(float value)
    {
        MinValue = value;
        CalculateBarFillAmount();
    }

    public void SetCurrentValue(float value)   
    {
        CurrentValue = value;
        CalculateBarFillAmount();
    }

    public void CalculateBarFillAmount()
    {
        // animation stuff here later
        Bar.fillAmount = Bastor.Helpers.GetPercentageBetweenValues(CurrentValue, MinValue, MaxValue);
    }
}
