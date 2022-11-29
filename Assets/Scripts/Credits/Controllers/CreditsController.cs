
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {
    [Header("Settings")]
    public ui_fade fade;
    public Animator camAnimator;

    public void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        this.camAnimator.SetBool("enable", true);
    }

    public void OnUIClick(string element) {
        if(element != "ui-btn-mainmenu") return;

        this.fade.fadeIn = true;
        this.fade.fadeDelay = 0f;
        this.fade.play();
        this.fade.OnFadeComplete += (bool fadein) => {
            if(!fadein) return;
            SceneManager.LoadScene("mainmenu", LoadSceneMode.Single);
        };
    }
}
