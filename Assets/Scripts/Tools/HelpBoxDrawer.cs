#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

// Adapted from: http://diegogiacomelli.com.br/unitytips-helpbox-attribute/

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxDrawer : PropertyDrawer {
    const float XPadding = 30f;
    const float YPadding = 5f;
    const float DefaultHeight = 20f;
    float _height;

    public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label) {

        var attr = attribute as HelpBoxAttribute;
        CalculateHeight(attr);

        EditorGUI.PropertyField(position, property, label, true);

        position = new Rect(
            XPadding,
            position.y + EditorGUI.GetPropertyHeight(property, label, true) + YPadding,
            position.width - XPadding,
            _height);

        EditorGUI.HelpBox(position, attr.Text, (MessageType) attr.Type);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true) + _height + 10;
    }

    void CalculateHeight(HelpBoxAttribute attr) {
        _height = (attr.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Length + 1) * DefaultHeight;
    }
}
#endif