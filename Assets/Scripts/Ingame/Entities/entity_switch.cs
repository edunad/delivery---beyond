
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
[RequireComponent(typeof(BoxCollider))]
public class entity_switch : MonoBehaviour {

    [Header("Settings")]
    public bool locked = false;

   #region EVENTS
        public delegate void onUSE(bool isDown);
        public event onUSE OnUSE;
    #endregion

    #region PRIVATE
        private entity_movement _movement;
        private BoxCollider _collider;
        private bool _isPressing;
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._movement = GetComponent<entity_movement>();
        this._collider = GetComponent<BoxCollider>();
        this._collider.isTrigger = true;

        if(!this.name.StartsWith("switch-")) this.name = "switch-" + this.name;
        this.gameObject.layer = 6;

        this._isPressing = false;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Switch/switch_on"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Switch/switch_off"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/btn_locked"),
        };
    }

    public void onPress(entity_player ply) {
        if(this._isPressing) return;

        if(this.locked) {
            SoundController.Instance.Play3DSound(this._audioClips[2], this.transform, Random.Range(0.85f, 1.15f), 3, 0.85f);
            return;
        }

        this._isPressing = true;
        this._movement.reverse = false;
        this._movement.start();

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform, Random.Range(0.85f, 1.15f), 3, 0.85f);
        if(this.OnUSE != null) this.OnUSE.Invoke(true);
    }

    public void onUseUP() {
        if(!this._isPressing) return;

        this._isPressing = false;
        this._movement.reverse = true;
        this._movement.start();

        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform, Random.Range(0.85f, 1.15f), 3, 0.85f);
        if(this.OnUSE != null) this.OnUSE.Invoke(false);
    }
}
