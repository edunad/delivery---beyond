
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class ui_jumpscare : MonoBehaviour {

    [Header("Settings")]
    public float speed = 0.1f;

    #region PRIVATE
        private AudioSource _audio;
        private SpriteRenderer _sprite;
        private Animator _animator;
    #endregion

    public void Awake() {
        this._animator = GetComponent<Animator>();

        this._sprite = GetComponent<SpriteRenderer>();
        this._sprite.enabled = false;

        this._audio = GetComponent<AudioSource>();
        this._audio.playOnAwake = false;
        this._audio.volume = 1f;
        this._audio.outputAudioMixerGroup = null;
    }

    public void jumpscare() {
        this._sprite.enabled = true;

        this._animator.SetBool("start", true);
        this._audio.Play();
    }
}
