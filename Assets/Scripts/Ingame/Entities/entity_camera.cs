
using UnityEngine;

public class entity_camera : MonoBehaviour {

    [Header("Settings")]
    public GameObject cameraBone;
    public float rotationAngle = 25f;
    public float rotationSpeed = 0.5f;

    #region PRIVATE
        private Vector3 _startAngle;
    #endregion

    public void Awake() {
        this._startAngle = this.cameraBone.transform.localEulerAngles;
    }

    public void Update() {
        Vector3 angles = this.cameraBone.transform.localEulerAngles;
        angles.y = this._startAngle.y +  Mathf.Cos(Time.time * rotationSpeed) * rotationAngle;

        this.cameraBone.transform.localEulerAngles = angles;
    }
}
