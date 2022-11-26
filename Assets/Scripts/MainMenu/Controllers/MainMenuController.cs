
using UnityEngine;

[DisallowMultipleComponent]
public class MainMenuController : MonoBehaviour {

    [Header("Settings")]
    public Camera sceneCamera;

    #region PRIVATE
    #endregion
	public static MainMenuController Instance;

    public MainMenuController() { Instance = this; }
    public void Awake() {

	}

    public void Update() {
        if(this.sceneCamera == null) return;

        float angleY = Mathf.Cos(Time.time * 0.25f) * 5f;
        this.sceneCamera.transform.rotation = Quaternion.Euler(new Vector3(0, angleY + 38f, 0));
    }

    public void OnUIClick(string id) {
        if(id == "new_game") {

        } else if(id == "options") {

        } else if(id == "quit") {
            Application.Quit();
        }
    }
}
