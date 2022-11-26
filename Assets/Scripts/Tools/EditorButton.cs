

#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorButton {
    public readonly string name;
    public readonly MethodInfo method;
    public readonly EditorButtonAttribute attr;

    public EditorButton(MethodInfo method, EditorButtonAttribute buttonAttribute) {
        this.method = method;
        this.attr = buttonAttribute;

        this.name = string.IsNullOrEmpty(buttonAttribute.name)
            ? ObjectNames.NicifyVariableName(method.Name)
            : buttonAttribute.name;
    }

    internal void Draw(IEnumerable<object> targets) {
        if (!GUILayout.Button(this.name)) return;

        foreach (object target in targets) {
            this.method.Invoke(target, null);
        }
    }
}
#endif