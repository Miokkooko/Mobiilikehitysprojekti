
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PassiveType { Damage, Firerate, Speed, MaxHealth }
[CreateAssetMenu(fileName = "PassiveData", menuName = "Passives/PassiveData", order = 1)]
public class PassiveData : StatusEffect
{
    public StatModifier BaseModifier;
    public float[] Upgrades;
    public PassiveType passiveType;
}

#if UNITY_EDITOR


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
#endif