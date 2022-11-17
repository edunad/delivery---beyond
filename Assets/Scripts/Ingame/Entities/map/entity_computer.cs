
using UnityEngine;

public class entity_computer : MonoBehaviour {
    [Header("Settings")]
    public Camera screenCamera;

    private entity_player _user;

    public void Awake() {
        this.screenCamera.enabled = false;
    }

    public bool isInUse() {return this._user != null;}
    public void onButtonPress(entity_player ply) { this.enter(ply); }
    private void enter(entity_player ply) {
        if(this.isInUse()) return;

        this.screenCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

        this._user = ply;
        this._user.freeze(true);
    }

    private void exit() {
        if(!this.isInUse()) return;

        this.screenCamera.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

        this._user.freeze(false);
        this._user = null;
    }

    public void Update() {
        if(this.isInUse() && Input.GetKeyDown(KeyCode.Escape)) {
            this.exit();
            return;
        }
    }
}
