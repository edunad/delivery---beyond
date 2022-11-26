#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public class EditorButtonDrawer {
    public readonly List<EditorButton> buttons;

    public EditorButtonDrawer(object target) {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        MethodInfo[] methods = target.GetType().GetMethods(flags);

        this.buttons = new List<EditorButton>();

        foreach (MethodInfo method in methods) {
            var buttonAttribute = method.GetCustomAttribute<EditorButtonAttribute>();
            if (buttonAttribute == null)
                continue;

            buttons.Add(new EditorButton(method, buttonAttribute));
        }
    }

    public void Draw(IEnumerable<object> targets) {
        foreach (var button in this.buttons)  {
            using (new EditorGUILayout.HorizontalScope()) {
                button.Draw(targets);
            }
        }
    }
}
#endif