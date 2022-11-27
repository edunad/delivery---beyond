
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ui_fade : MonoBehaviour {

    [Header("Fade settings")]
    public bool fadeIn;
    public float fadeSpeed;
    public float fadeDelay;
    public bool playOnAwake;

    #region PRIVATE
        private float _fadeTime;
        private float _timer;
        private SpriteRenderer _sprite;
        private bool _playing;
        private float _alpha;
    #endregion

    #region EVENTS
        public delegate void onFadeComplete(bool fadeIn);
        public event onFadeComplete OnFadeComplete;
    #endregion

    public void Awake () {
        this._sprite = GetComponent<SpriteRenderer>();
        this._sprite.enabled = true;

        if(this.playOnAwake) this.play();
    }

    public void play() {
        this.setAlpha(this.fadeIn ? 0f : 1f);

        this._playing = true;
        this._fadeTime = Time.time + fadeDelay;
        this._timer = 0;
        this._alpha = this._sprite.color.a;
    }

    public void Update() {
        if (this._sprite == null || !this._playing) return;
        if (Time.time < this._fadeTime) return;

        this.setAlpha(fadeIn ?
            Mathf.Lerp(this._alpha, 1f, this._timer) :
            Mathf.Lerp(this._alpha, 0f, this._timer));

        this._timer += this.fadeSpeed * Time.deltaTime;
        if(this._timer >= 1f) {
            this._playing = false;

            this.setAlpha(fadeIn ? 1f: 0f);
            if(OnFadeComplete != null) OnFadeComplete.Invoke(fadeIn);
        }
    }

    private void setAlpha(float alpha) {
        Color col = this._sprite.color;
        col.a = alpha;
        this._sprite.color = col;
    }
}
