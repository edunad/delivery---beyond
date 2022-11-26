
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class util_aspectratio : MonoBehaviour {

    [Header("Settings")]
    public float WIDTH = 1920;
    public float HEIGHT = 1080;

    #region PRIVATE
        private Camera _cam;
        private float _defaultZoom;
    #endregion

    public void Awake () {
        this._cam = GetComponent<Camera>();
        this._defaultZoom = this._cam.orthographicSize;
	}

    public void Update() {
        if(this._cam == null) return;

        float expectedRatio = WIDTH / HEIGHT;
        float currentRatio = (float)this._cam.pixelWidth / (float)this._cam.pixelHeight;

        // ok whatever it works
        this._cam.orthographicSize = (this._defaultZoom * 2) - ((currentRatio * this._defaultZoom) / expectedRatio);
    }
}
