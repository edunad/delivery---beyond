
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverController : MonoBehaviour {
    [Header("Settings")]
    public ui_fade fade;

    public void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OnUIClick(string element) {
        if(element != "ui-btn-retry") return;

        this.fade.fadeIn = true;
        this.fade.fadeDelay = 0f;
        this.fade.play();
        this.fade.OnFadeComplete += (bool fadein) => {
            if(!fadein) return;
            PlayerPrefs.SetInt("loading_scene_index", PlayerPrefs.GetInt("current_scene", 1));
            SceneManager.LoadScene("loading", LoadSceneMode.Single);
        };
    }
}
