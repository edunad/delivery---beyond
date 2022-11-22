
using UnityEngine;

[DisallowMultipleComponent]
public class UIController : MonoBehaviour {
	public static UIController Instance;

    #region PRIVATE
        private Camera _uiCamera;
    #endregion

    public UIController() { Instance = this; }
    public void Awake() {
        this._uiCamera = GetComponent<Camera>();
	}
}
