
#if UNITY_EDITOR
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Object), true), CanEditMultipleObjects]
public class DEVEditor : Editor {
    private EditorButtonDrawer _buttons;

    private void OnEnable() {
        this._buttons = new EditorButtonDrawer(target);
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        this._buttons.Draw(targets);
    }
}
#endif