
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class ui_jumpscare : MonoBehaviour {

    [Header("Settings")]
    public float speed = 0.1f;

    #region PRIVATE
        private AudioSource _audio;
        private SpriteRenderer _sprite;
        private bool _enabled = false;
    #endregion

    public void Awake() {
        this._sprite = GetComponent<SpriteRenderer>();
        this._sprite.enabled = false;

        this._audio = GetComponent<AudioSource>();
        this._audio.playOnAwake = false;

        this._enabled = false;
    }

    public void jumpscare() {
        this._enabled = true;

        this._sprite.enabled = true;
        this._audio.Play();
    }

    public void Update() {
        if(!this._enabled) return;
        this._sprite.transform.localScale = this._sprite.transform.localScale + new Vector3(speed, speed, speed) * Time.deltaTime;
    }
}
