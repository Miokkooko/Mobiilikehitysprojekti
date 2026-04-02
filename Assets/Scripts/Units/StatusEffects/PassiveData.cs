using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "PassiveData", menuName = "Passives/PassiveData", order = 1)]
public class PassiveData : StatusEffect
{
    public Sprite Icon;
    public StatModifier BaseModifier;
    public float[] Upgrades;
}

[CustomEditor(typeof(PassiveData)), CanEditMultipleObjects]
public class PassiveDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // update the current values into the serialized object and propreties
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "Duration", "MaxStacks", "StackType", "LifetimeType", "ModifierType");

        serializedObject.ApplyModifiedProperties();
    }
}