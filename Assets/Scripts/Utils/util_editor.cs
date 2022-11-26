
#if UNIT_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

[InitializeOnLoad]
public class EditorCoroutine {
    private static List<IEnumerator> _coroutines = new List<IEnumerator>();
    private static int _current = 0;

    static EditorCoroutine() {
        EditorApplication.update += EditorCoroutine.onEditorUpdate;
    }

    public static IEnumerator addCoroutine(IEnumerator newCorou) {
        _coroutines.Add(newCorou);
        return newCorou;
    }

    private static void onEditorUpdate() {
        if(_coroutines.Count <= 0) return;

        _current = (_current + 1) % _coroutines.Count;
        if(!_coroutines[_current].MoveNext()) _coroutines.RemoveAt(_current);
    }
}
#endif