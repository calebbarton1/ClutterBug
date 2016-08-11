using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

    public class HideInSubClassAttribute : PropertyAttribute { }
    public class HideInParentClass : PropertyAttribute { }

[CustomPropertyDrawer(typeof(HideInSubClassAttribute))]
public class HideInSubClassAttributeDrawer : PropertyDrawer
{

    private bool ShouldShow(SerializedProperty property)
    {
        Type type = property.serializedObject.targetObject.GetType();
        FieldInfo field = type.GetField(property.name);
        Type declaringType = field.DeclaringType;
        return type == declaringType;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            EditorGUI.PropertyField(position, property); //fun fact: base.OnGUI doesn't work! Check for yourself!
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }

    [CustomPropertyDrawer(typeof(HideInParentClass))]
    public class HideInParentClassAttributeDrawer : PropertyDrawer
    {

        private bool ShouldShow(SerializedProperty property)
        {
            Type type = property.serializedObject.targetObject.GetType();
            FieldInfo field = type.GetField(property.name);
            Type declaringType = field.DeclaringType;
            return type != declaringType;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
                EditorGUI.PropertyField(position, property); //fun fact: base.OnGUI doesn't work! Check for yourself!
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
                return base.GetPropertyHeight(property, label);
            else
                return 0;
        }
    }
}
#endif
