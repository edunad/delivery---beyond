
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MainMenuController : MonoBehaviour {

    [Header("Settings")]
    public Camera sceneCamera;
    public ui_fade fade;

    public void Awake() {
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
    }

    public void Update() {
        if(this.sceneCamera == null) return;

        float angleY = Mathf.Cos(Time.time * 0.25f) * 5f;
        this.sceneCamera.transform.rotation = Quaternion.Euler(new Vector3(0, angleY + 38f, 0));
    }

    public void OnUIClick(string id) {
        if(id == "newgame") {
            this.fade.fadeIn = true;
            this.fade.fadeDelay = 0f;
            this.fade.play();
            this.fade.OnFadeComplete += (bool fadein) => {
                if(!fadein) return;

                PlayerPrefs.SetInt("loading_scene_index", SceneManager.GetActiveScene().buildIndex + 2);
                SceneManager.LoadScene("loading", LoadSceneMode.Single);
            };
        } else if(id == "options") {
            OptionsController.Instance.displayOptions(true);
        } else if(id == "quit") {
            Application.Quit();
        }
    }
}
