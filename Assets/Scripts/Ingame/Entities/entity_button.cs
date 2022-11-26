
using UnityEngine;

public enum MoveDirection {
    NONE = 0,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    FRONT,
    BACK
}

[RequireComponent(typeof(Collider))]
public class entity_button : MonoBehaviour {

    [Header("Sound")]
    public string buttonLocked = "btn_locked";
    public string buttonOK = "btn_ok";

    [Header("Settings")]
    public bool locked;
    public MoveDirection moveDirection = MoveDirection.DOWN;
    public float moveDistance = 0.4f;

    public float resetCooldown = 2f;

    public delegate void onUSE(entity_player ply);
    public event onUSE OnUSE;

    // PRIVATE ---
    private AudioClip[] _audioClips;

    private Collider _collision;
    private Vector3 _originalPos;

    public void Awake() {
        this.gameObject.isStatic = false;

        this._collision = GetComponent<Collider>();
        this._collision.isTrigger = true;

        if(!this.name.StartsWith("btn-")) this.name = "btn-" + this.name;
        this.gameObject.layer = 6;

        this._originalPos = transform.localPosition;
        this.setButtonLocked(this.locked, true);

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/" + this.buttonLocked),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/" + this.buttonOK)
        };
    }

    public void setButtonLocked(bool locked, bool force = false) {
        if(!force && this.locked == locked) return;

        if(!locked){
            this.locked = false;
            this.transform.localPosition = this._originalPos;
        } else {
            this.locked = true;
            this.moveButton(this.moveDirection, this.moveDistance);
        }
    }

    public void onPlayerUse(entity_player ply) {
        if(ply.isHoldingItem()) return;

        if(this.locked) {
            SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);
            return;
        }

        this.setButtonLocked(true);
        if(OnUSE != null) OnUSE.Invoke(ply);

        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform);
        if(this.resetCooldown > 0) util_timer.simple(this.resetCooldown, () => {
            this.setButtonLocked(false);
        });
    }

    private void moveButton(MoveDirection direction, float distance) {
        Vector3 ps = this.transform.localPosition;

        switch(direction) {
            case MoveDirection.DOWN:
                ps.y += -distance;
                break;
            case MoveDirection.UP:
                ps.y += distance;
                break;
            case MoveDirection.LEFT:
                ps.x += distance;
                break;
            case MoveDirection.RIGHT:
                ps.x += -distance;
                break;
            case MoveDirection.FRONT:
                ps.z += distance;
                break;
            case MoveDirection.BACK:
                ps.z += -distance;
                break;
        }

        this.transform.localPosition = ps;
    }

}
