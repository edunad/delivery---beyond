
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class entity_blink : MonoBehaviour {

    [Header("Settings")]
    public float blinkSpeed;

    #region PRIVATE
        private SpriteRenderer _renderer;
        private util_timer _timer;
    #endregion

    public void Awake() {
        this._renderer = GetComponent<SpriteRenderer>();
    }

    public void OnEnable() {
        if(this._timer != null) this._timer.stop();
        this._timer = util_timer.create(-1, this.blinkSpeed, () => {
            this._renderer.enabled = !this._renderer.enabled;
        });
    }

    public void OnDisable() {
        if(this._timer != null) this._timer.stop();
        this._timer = null;
        this._renderer.enabled = false;
    }

}
