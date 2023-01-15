
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

[DisallowMultipleComponent]
public class TutorialController : MonoBehaviour {
    public static TutorialController Instance { get; private set; }

    [Header("Settings")]
    public List<GameObject> tutorialAreas = new List<GameObject>();

    #region PRIVATE
        private Queue<GameObject> _tutorialQueue;
        private GameObject _prevZone;
    #endregion

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        this._tutorialQueue = new Queue<GameObject>(tutorialAreas);

        util_timer.simple(2f,() => {
            this.activateNextArea();
        });
    }

    public entity_tutorial_zone activateNextArea() {
        if(this._tutorialQueue.Count <= 0) {
            Debug.Log("Tutorial done");

            PlayerPrefs.SetInt("loading_scene_index", SceneManager.GetActiveScene().buildIndex + 2);
            SceneManager.LoadScene("loading", LoadSceneMode.Single);
            return null;
        } else {
            GameObject newZone = this._tutorialQueue.Dequeue();

            entity_tutorial_zone zone = newZone.GetComponent<entity_tutorial_zone>();
            if(zone == null) throw new System.Exception("Invalid tutorial object, missing entity_tutorial_zone");

            newZone.SetActive(true);

            if(this._prevZone != null) this._prevZone.SetActive(false);
            this._prevZone = newZone;

            zone.activateArea();
            return zone;
        }
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void OnGUI() {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        string data = util_timer.debug();
        data += "\n\n" + util_fade_timer.debug();

        GUI.Label(rect, data, style);
    }
#endif

    public void FixedUpdate() {
        util_timer.fixedUpdate();
        util_fade_timer.fixedUpdate();
    }

    public void OnDestroy() {
        util_timer.clear();
        util_fade_timer.clear();
    }
}
