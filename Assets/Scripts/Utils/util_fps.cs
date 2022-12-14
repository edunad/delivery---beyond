
using UnityEngine;

public class util_fps : MonoBehaviour {
    #if UNITY_EDITOR
        float deltaTime = 0.0f;

        public void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        public void OnGUI() {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;

            GUI.Label(rect, string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps), style);
        }
    #else
        public void Awake() {
            Destroy(this);
        }
    #endif
}