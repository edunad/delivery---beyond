
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIController : MonoBehaviour {
	public static UIController Instance;

    private Camera _uiCamera;

    public void Awake() {
		if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        this._uiCamera = GetComponent<Camera>();
	}
}
