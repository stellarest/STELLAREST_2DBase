using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string valueStr = string.Empty;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = $"[Integer]: {property.intValue.ToString()}";
                break;

            case SerializedPropertyType.Boolean:
                valueStr = $"[Boolean]: {property.boolValue.ToString()}";
                break;
            
            case SerializedPropertyType.Float:
                valueStr = $"[Float]: {property.floatValue.ToString("0.00000")}";
                break;
            
            case SerializedPropertyType.String:
                valueStr = $"{property.stringValue}";
                break;

            default:
                valueStr = ": (not supported)";
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}
