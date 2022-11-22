
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
[RequireComponent(typeof(entity_teleport))]
public class entity_elevator : MonoBehaviour {

    public entity_button button;

    #region PRIVATE
        private entity_teleport _teleport;
        private entity_movement _elevatorGate;
        private entity_player _ply;
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._teleport = GetComponent<entity_teleport>();

        this._elevatorGate = GetComponent<entity_movement>();
        this._elevatorGate.reverse = false;
        this._elevatorGate.OnMovementFinish += this.onGateMovementFinish;

        this.button.OnUSE += onElevatorButtonUSE;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_start"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_moving"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_bell"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_stop"),
        };
    }

    public void onElevatorButtonUSE(entity_player ply) {
        this._ply = ply;

        this._elevatorGate.reverse = false;
        this._elevatorGate.start();
    }

    private void onGateMovementFinish(bool isReverse) {
        if(isReverse){
            // Reset Elevator
            button.setButtonLocked(false);
            this._ply = null;
        } else {
            util_timer.simple(0.25f, () => {
                //elevator move sound

                this._ply.shakeCamera(12f);
                this._teleport.teleport();

                SoundController.Instance.Play3DSound(this._audioClips[0], this._teleport.teleportDest);
                SoundController.Instance.Play3DSound(this._audioClips[1], this._teleport.teleportDest);

                util_timer.simple(12f, () => {
                    // elevator DING sound
                    SoundController.Instance.Play3DSound(this._audioClips[2], this._teleport.teleportDest);
                    SoundController.Instance.Play3DSound(this._audioClips[3], this._teleport.teleportDest);

                    this._elevatorGate.reverse = true;
                    this._elevatorGate.start();
                });
            });
        }
    }
}
