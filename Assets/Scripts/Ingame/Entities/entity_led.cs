
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class entity_led : MonoBehaviour {

    [Header("Settings")]
    public Color activeColor;
    public Color disabledColor;
    public bool active;

    #region PRIVATE
        private MeshRenderer _renderer;
    #endregion

    public void Awake() {
        this._renderer = GetComponent<MeshRenderer>();
        this.setActive(this.active);
    }

    public void setActive(bool active) {
        this.active = active;
        this._renderer.material.SetColor("_Color", active ? this.activeColor : this.disabledColor);
    }
}
