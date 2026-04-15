using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BehindBarAnimationMode { FadeOut, Lerp }
public enum DifferenceDisplayMode { Cumulative, Latest }

public class BarUIElement : MonoBehaviour
{
    public SlicedFilledImage mainBar;

    [SerializeField] float maxValue;
    [SerializeField] float minValue = 0;
    [SerializeField] float currentValue;
    float GetBarFillAmount => Bastor.Helpers.GetPercentageBetweenValues(currentValue, minValue, maxValue);
    float GetBehindBarFillAmount => Bastor.Helpers.GetPercentageBetweenValues(previousValue, minValue, maxValue);

    float ConvertFillAmountToValue(float value) => value * maxValue;
    float previousValue;

    [Header("Decrease Animations")]
    public SlicedFilledImage behindBar;

    public BehindBarAnimationMode behindBarAnimationMode;
    public DifferenceDisplayMode behindBarDisplayMode;
    public LeanTweenType behindBarTweenType = LeanTweenType.linear;

    public float animationDelay = 1;
    public float animationDuration = 1;

    int behindBarTweenId = -1;
    int delayTweenId = -1;

    private void Start()
    {
        if (mainBar == null)
            Debug.LogError("Main Bar SlicedFilledImage missing from BarUIElement!");
        if (behindBar == null)
            Debug.LogWarning("BehindBar is not assigned to BarUIElement. If this is intentional, decrease animations are disabled.");

        if (mainBar != null)
            CalculateBarFillAmount();
    }

    public void SetMaxValue(float value) 
    {
        maxValue = value;
        CalculateBarFillAmount();
    }

    public void SetMinValue(float value)
    {
        minValue = value;
        CalculateBarFillAmount();
    }

    public void SetCurrentValue(float value)   
    {
        switch (behindBarDisplayMode)
        {
            case DifferenceDisplayMode.Cumulative:
                if (previousValue <= currentValue)
                    previousValue = currentValue;
                break;
            case DifferenceDisplayMode.Latest:
                previousValue = currentValue;
                break;
        }
        
        currentValue = value;
        CalculateBarFillAmount();
    }

    public void CalculateBarFillAmount()
    {
        if (mainBar == null)
            return;

        mainBar.fillAmount = GetBarFillAmount;

        if (behindBar == null)
            return;

        // If we gained value, then just fill the behindbar to the current value and exit
        if (mainBar.fillAmount >= behindBar.fillAmount)
        {
            behindBar.fillAmount = mainBar.fillAmount;
            return;
        }

        behindBar.fillAmount = GetBehindBarFillAmount;

        switch (behindBarAnimationMode)
        {
            case BehindBarAnimationMode.FadeOut:
                FadeBehindBar();
                break;
            case BehindBarAnimationMode.Lerp:
                TweenBehindBar();
                break;
            default:
                break;
        }
    }

    void TweenBehindBar()
    {
        if (LeanTween.isTweening(delayTweenId))
            LeanTween.cancel(delayTweenId);

        if (LeanTween.isTweening(behindBarTweenId))
            LeanTween.cancel(behindBarTweenId);
        
        delayTweenId = LeanTween.delayedCall(gameObject, animationDelay, () =>
        {
            behindBarTweenId = LeanTween.value(gameObject, GetBehindBarFillAmount, GetBarFillAmount, animationDuration)
                .setEase(behindBarTweenType)
                .setOnUpdate((val) =>
                {
                    behindBar.fillAmount = val;
                    previousValue = ConvertFillAmountToValue(val);
                })
                .setOnComplete(() =>
                {
                    previousValue = currentValue;
                })
                .id;
        }).id;
    }

    void FadeBehindBar()
    {
        if (LeanTween.isTweening(delayTweenId))
            LeanTween.cancel(delayTweenId);

        if (LeanTween.isTweening(behindBarTweenId))
            LeanTween.cancel(behindBarTweenId);

        float alpha = 1f;
        behindBar.color = new Color(behindBar.color.r, behindBar.color.g, behindBar.color.b, alpha);
       
        delayTweenId = LeanTween.delayedCall(gameObject, animationDelay, () =>
        {
            behindBarTweenId = LeanTween.value(gameObject, alpha, 0, animationDuration)
               .setEase(behindBarTweenType)
               .setOnUpdate(val => behindBar.color = new Color(behindBar.color.r, behindBar.color.g, behindBar.color.b, val))
               .setOnComplete(() =>
               {
                    previousValue = currentValue;
               })
               .id;
        }).id;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(BarUIElement))]
public class BarUIElementEditor : Editor
{
    SerializedProperty mainBar;

    SerializedProperty maxValue;
    SerializedProperty minValue;
    SerializedProperty currentValue;

    SerializedProperty behindBar;

    SerializedProperty behindBarAnimationMode;
    SerializedProperty barDisplayMode;
    SerializedProperty tweeningType;
    SerializedProperty animationDelay;
    SerializedProperty animationDuration;

    void OnEnable()
    {
        mainBar = serializedObject.FindProperty("mainBar");

        maxValue = serializedObject.FindProperty("maxValue");
        minValue = serializedObject.FindProperty("minValue");
        currentValue = serializedObject.FindProperty("currentValue");

        behindBar = serializedObject.FindProperty("behindBar");

        behindBarAnimationMode = serializedObject.FindProperty("behindBarAnimationMode");
        barDisplayMode = serializedObject.FindProperty("behindBarDisplayMode");
        tweeningType = serializedObject.FindProperty("behindBarTweenType");
        animationDelay = serializedObject.FindProperty("animationDelay");
        animationDuration = serializedObject.FindProperty("animationDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mainBar);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(maxValue);
        EditorGUILayout.PropertyField(minValue);
        EditorGUILayout.PropertyField(currentValue);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(behindBar);

        if (behindBar.objectReferenceValue != null)
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(behindBarAnimationMode);
            EditorGUILayout.PropertyField(barDisplayMode);
            EditorGUILayout.PropertyField(tweeningType);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(animationDelay);
            EditorGUILayout.PropertyField(animationDuration);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif