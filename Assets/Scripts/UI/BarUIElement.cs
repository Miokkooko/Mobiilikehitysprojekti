using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BehindBarAnimationMode { FadeOut, Lerp }
public enum BarAnimationMode { Instant, Lerp }
public enum DifferenceDisplayMode { Cumulative, Latest }

public class BarUIElement : MonoBehaviour
{
    public SlicedFilledImage mainBar;
    public BarAnimationMode animationMode = BarAnimationMode.Instant;
    public LeanTweenType tweenType = LeanTweenType.linear;

    [SerializeField] float maxValue;
    [SerializeField] float minValue = 0;
    [SerializeField] float currentValue;
    float GetBarFillAmount => Bastor.Helpers.GetPercentageBetweenValues(currentValue, minValue, maxValue);
    float GetBehindBarFillAmount => Bastor.Helpers.GetPercentageBetweenValues(behindBarPreviousValue, minValue, maxValue);

    float ConvertFillAmountToValue(float value) => value * maxValue;
    float behindBarPreviousValue;

    [Header("Decrease Animations")]
    public SlicedFilledImage behindBar;

    public BehindBarAnimationMode behindBarAnimationMode;
    public DifferenceDisplayMode behindBarDisplayMode;

    public float animationDelay = 1;
    public float animationDuration = 1;

    int behindBarTweenId = -1;
    int barTweenId = -1;
    int behindBarDelayTweenId = -1;

    public Action BarFilled;
    public Action BarDepleted;

    private void Start()
    {
        if (mainBar == null)
            Debug.LogError("Main Bar SlicedFilledImage missing from BarUIElement!");
        if (behindBar == null)
            //Debug.LogWarning("BehindBar is not assigned to BarUIElement. If this is intentional, decrease animations are disabled.");

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
                if (behindBarPreviousValue <= currentValue)
                    behindBarPreviousValue = currentValue;
                break;
            case DifferenceDisplayMode.Latest:
                behindBarPreviousValue = currentValue;
                break;
        }

        currentValue = value;
        CalculateBarFillAmount();
    }
    public void SetCurrentValueInstant(float value)
    {
        switch (behindBarDisplayMode)
        {
            case DifferenceDisplayMode.Cumulative:
                if (behindBarPreviousValue <= currentValue)
                    behindBarPreviousValue = currentValue;
                break;
            case DifferenceDisplayMode.Latest:
                behindBarPreviousValue = currentValue;
                break;
        }

        currentValue = value;

        BarAnimationMode oldMode = animationMode;
        animationMode = BarAnimationMode.Instant;
        CalculateBarFillAmount();
        animationMode = oldMode;
    }

    void CheckEvents()
    {
        if (mainBar.fillAmount == 1)
            BarFilled?.Invoke();
        else if (mainBar.fillAmount == 0)
            BarDepleted?.Invoke();
    }

    public void CalculateBarFillAmount()
    {
        if (mainBar == null)
            return;

        switch (animationMode)
        {
            case BarAnimationMode.Instant:
                if (LeanTween.isTweening(barTweenId))
                    LeanTween.cancel(barTweenId);
                mainBar.fillAmount = GetBarFillAmount;
                CheckEvents();
                break;
            case BarAnimationMode.Lerp:
                TweenBar(mainBar.fillAmount, GetBarFillAmount);
                break;
        }

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
                TweenBehindBar(GetBehindBarFillAmount, GetBarFillAmount, animationDelay);
                break;
            default:
                break;
        }
    }
    void TweenBar(float from, float to)
    {
        if (LeanTween.isTweening(barTweenId))
            LeanTween.cancel(barTweenId);

        barTweenId = LeanTween.value(gameObject, from, to, animationDuration)
            .setEase(tweenType)
            .setOnUpdate((val) =>
            {
                mainBar.fillAmount = val;
                CheckEvents();
            })
            .id;
    }

    void TweenBehindBar(float from, float to, float delay)
    {
        if (LeanTween.isTweening(behindBarDelayTweenId))
            LeanTween.cancel(behindBarDelayTweenId);

        if (LeanTween.isTweening(behindBarTweenId))
            LeanTween.cancel(behindBarTweenId);

        behindBarDelayTweenId = LeanTween.delayedCall(gameObject, delay, () =>
        {
            behindBarTweenId = LeanTween.value(gameObject, from, to, animationDuration)
                .setEase(tweenType)
                .setOnUpdate((val) =>
                {
                    behindBar.fillAmount = val;
                    behindBarPreviousValue = ConvertFillAmountToValue(val);
                })
                .setOnComplete(() =>
                {
                    behindBarPreviousValue = currentValue;
                })
                .id;
        }).id;
    }



    void FadeBehindBar()
    {
        if (LeanTween.isTweening(behindBarDelayTweenId))
            LeanTween.cancel(behindBarDelayTweenId);

        if (LeanTween.isTweening(behindBarTweenId))
            LeanTween.cancel(behindBarTweenId);

        behindBar.color = new Color(behindBar.color.r, behindBar.color.g, behindBar.color.b, 1);

        behindBarDelayTweenId = LeanTween.delayedCall(gameObject, animationDelay, () =>
        {
            behindBarTweenId = LeanTween.value(gameObject, 1, 0, animationDuration)
               .setEase(tweenType)
               .setOnUpdate(val => behindBar.color = new Color(behindBar.color.r, behindBar.color.g, behindBar.color.b, val))
               .setOnComplete(() =>
               {
                   behindBarPreviousValue = currentValue;
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
    SerializedProperty animationMode;

    SerializedProperty tweenType;
    SerializedProperty maxValue;
    SerializedProperty minValue;
    SerializedProperty currentValue;

    SerializedProperty behindBar;

    SerializedProperty behindBarAnimationMode;
    SerializedProperty barDisplayMode;

    SerializedProperty animationDelay;
    SerializedProperty animationDuration;

    void OnEnable()
    {
        mainBar = serializedObject.FindProperty("mainBar");
        animationMode = serializedObject.FindProperty("animationMode");

        maxValue = serializedObject.FindProperty("maxValue");
        minValue = serializedObject.FindProperty("minValue");
        currentValue = serializedObject.FindProperty("currentValue");

        behindBar = serializedObject.FindProperty("behindBar");

        behindBarAnimationMode = serializedObject.FindProperty("behindBarAnimationMode");
        barDisplayMode = serializedObject.FindProperty("behindBarDisplayMode");
        tweenType = serializedObject.FindProperty("tweenType");
        animationDelay = serializedObject.FindProperty("animationDelay");
        animationDuration = serializedObject.FindProperty("animationDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mainBar);
        EditorGUILayout.PropertyField(animationMode);
        EditorGUILayout.PropertyField(tweenType);
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


            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(animationDelay);
            EditorGUILayout.PropertyField(animationDuration);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif