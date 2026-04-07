using System;
using UnityEditor;
using UnityEngine;

public enum StatType
{
    Damage,
    Speed,
    MaxHealth,
    Piercing,
    ProjectileCount,
    Firerate,
    AoERadius,
    AoEDamage
}

[System.Serializable]
public class StatModifier
{
    public StatType Stat;
    public float Value;
    public ModifierType Type;
    public StatusEffect source;

 
}
public enum ModifierType
{
    Flat,       // +5 damage
    Percent,     // +20%
    None
}

[Serializable]
public class WeaponModifier : StatModifier
{
    public string upgradeDescription;
}

[CustomPropertyDrawer(typeof(WeaponModifier))]
public class WeaponModifierDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var prop = property.Copy();
        var end = prop.GetEndProperty();

        float y = position.y;

        while (prop.NextVisible(true) && !SerializedProperty.EqualContents(prop, end))
        {
            if (prop.name == "source")
                continue;

            float height = EditorGUI.GetPropertyHeight(prop, true);
            Rect fieldRect = new Rect(position.x, y, position.width, height);

            EditorGUI.PropertyField(fieldRect, prop, true);
            y += height + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float total = 0f;

        var prop = property.Copy();
        var end = prop.GetEndProperty();

        while (prop.NextVisible(true) && !SerializedProperty.EqualContents(prop, end))
        {
            if (prop.name == "source")
                continue;

            total += EditorGUI.GetPropertyHeight(prop, true);
            total += EditorGUIUtility.standardVerticalSpacing;
        }

        return total;
    }
}