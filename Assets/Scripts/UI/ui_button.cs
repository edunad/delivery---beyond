
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(TextMeshPro))]
public class ui_button : MonoBehaviour {

    [Header("Settings")]
    public string id;
    public GameObject targetObject;
    public float cooldown = 0.3f;
    public bool canHoldMouse = false;

    #region PRIVATE
        private float _tapCD;
        private bool _isEnabled;
        private bool _mouseDown;
        private TextMeshPro _text;
        private readonly Color32 _normalColor = new Color32(180, 180, 180, 255);
    #endregion

    public void Awake() {
        this._text = GetComponent<TextMeshPro>();

        this.setColor(this._normalColor);
        this._isEnabled = true;
    }

    public void OnMouseOver() {
        if (!this._isEnabled) return;
        this.setColor(Color.white);
    }

    public void OnMouseExit() {
        if (!this._isEnabled) return;
        this.setColor(this._normalColor);
    }

    public void OnMouseDown() {
        this._mouseDown = true;
    }

    public void OnMouseUp() {
        this._mouseDown = false;
    }

    public void Update() {
        if (!this._mouseDown || !this._isEnabled) return;
        if (this._tapCD > Time.time - this.cooldown) return;

        // Stop pressing
        if (!this.canHoldMouse) this._mouseDown = false;

        this._tapCD = Time.time; // Prevent spamming
        this.targetObject.SendMessage("OnUIClick", this.id, SendMessageOptions.DontRequireReceiver);
    }

    public void isEnabled(bool enabled) {
        this._isEnabled = enabled;
    }

    private void setColor(Color cl) {
        this._text.color = cl;
    }
}
