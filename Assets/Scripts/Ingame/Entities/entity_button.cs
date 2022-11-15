
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

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class entity_button : MonoBehaviour {

    public GameObject reciever;

    [Header("Sound")]
    public string buttonLocked = "btn_locked";
    public string buttonOK = "btn_ok";

    [Header("Settings")]
    public bool locked;
    public MoveDirection moveDirection = MoveDirection.DOWN;
    public float moveDistance = 0.4f;

    public float resetCooldown = 2f;

    // PRIVATE ---

    private Collider _collision;
    private AudioSource _audioSource;
    private AudioClip[] _audioClips;
    private Vector3 _originalPos;

    public void Awake() {
        this._collision = GetComponent<Collider>();
        this._collision.isTrigger = true;

        this.name = "entity_button";
        this.gameObject.layer = 6;

        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.volume = 0.4f;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/" + this.buttonLocked),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/" + this.buttonOK)
        };
    }

    private void moveButton(MoveDirection direction, float distance) {
        Vector3 ps = transform.localPosition;

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

        transform.localPosition = ps;
    }

    public bool onPlayerUse(entity_player ply) {
        if(this.reciever == null) throw new System.Exception("Invalid reciever");
        if(this.locked) {
            this._audioSource.clip = this._audioClips[0];
            this._audioSource.Play();

            return false;
        }

        this.locked = true;
        this.reciever.BroadcastMessage("onButtonPress", ply);

        this._originalPos = transform.localPosition;
        this.moveButton(this.moveDirection, this.moveDistance);

        this._audioSource.clip = this._audioClips[1];
        this._audioSource.Play();

        util_timer.Simple(this.resetCooldown, () => {
            this.locked = false;
            transform.localPosition = this._originalPos;
        });

        return true;
    }
}
