
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
[RequireComponent(typeof(entity_teleport))]
public class entity_elevator : MonoBehaviour {

    public entity_button button;
    public entity_elevator teleTo;

    [HideInInspector]
    public entity_movement elevatorGate;

    #region PRIVATE
        private entity_teleport _teleport;
        private entity_player _ply;
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._teleport = GetComponent<entity_teleport>();

        this.elevatorGate = GetComponent<entity_movement>();
        this.elevatorGate.reverse = false;
        this.elevatorGate.OnMovementFinish += this.onGateMovementFinish;

        this.button.OnUSE += onElevatorButtonUSE;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_start"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_moving"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_bell"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_stop"),
        };

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        this.button.setButtonLocked(newStatus != GAMEPLAY_STATUS.ITEM_RETRIEVE);
    }

    private void onElevatorButtonUSE(entity_player ply) {
        this._ply = ply;

        // Prepare the other elevator
        this.teleTo.elevatorGate.reverse = true;
        this.teleTo.elevatorGate.reset(); // Snap the other gate
        this.teleTo.button.setButtonLocked(true);
        // ---

        this.elevatorGate.reverse = false;
        this.elevatorGate.start();
    }

    private void onGateMovementFinish(bool isReverse) {
        if(isReverse) {
            // Reset Elevator
            button.setButtonLocked(false);
            this._ply = null;
        } else {
            util_timer.simple(0.25f, () => {
                this._ply.shakeCamera(12f);
                this._teleport.teleport(this.teleTo.transform);

                SoundController.Instance.Play3DSound(this._audioClips[0], this.teleTo.transform);
                SoundController.Instance.Play3DSound(this._audioClips[1], this.teleTo.transform);

                util_timer.simple(12f, () => {
                    // elevator DING sound
                    SoundController.Instance.Play3DSound(this._audioClips[2], this.teleTo.transform);
                    SoundController.Instance.Play3DSound(this._audioClips[3], this.teleTo.transform);

                    // Open both doors
                    this.elevatorGate.reverse = true;
                    this.elevatorGate.start();
                    this.teleTo.elevatorGate.reverse = true;
                    this.teleTo.elevatorGate.start();
                    // ----
                });
            });
        }
    }
}
