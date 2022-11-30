
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
[RequireComponent(typeof(entity_teleport))]
public class entity_elevator : MonoBehaviour {

    [Header("Settings")]
    public entity_elevator teleTo;
    public readonly float elevatorTime = 7f;

    [HideInInspector]
    public entity_movement elevatorGate;

    [HideInInspector]
    public entity_item_spot itemSpot;

    #region PRIVATE
        private entity_button _button;
        private entity_teleport _teleport;
        private entity_player _ply;
        private AudioClip[] _audioClips;
        private entity_sound _elevatorMoveSnd;
    #endregion

    public void Awake() {
        this._teleport = GetComponent<entity_teleport>();

        this.elevatorGate = GetComponent<entity_movement>();
        this.elevatorGate.reverse = false;
        this.elevatorGate.OnMovementFinish += this.onGateMovementFinish;

        this.itemSpot = GetComponentInChildren<entity_item_spot>(true);

        this._button = GetComponentInChildren<entity_button>(true);
        this._button.OnUSE += onElevatorButtonUSE;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_start"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_moving"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Elevator/elevator_stop"),
        };

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        this._button.setButtonLocked(newStatus != GAMEPLAY_STATUS.ITEM_RETRIEVE);
    }

    private void onElevatorButtonUSE(entity_player ply) {
        this._ply = ply;

        // Prepare the other elevator
        this.teleTo.elevatorGate.reverse = true;
        this.teleTo.elevatorGate.reset(); // Snap the other gate
        this.teleTo._button.setButtonLocked(true);
        // ---

        this.elevatorGate.reverse = false;
        this.elevatorGate.start();

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);
        SoundController.Instance.Play3DSound(this._audioClips[0], this.teleTo.transform);

        this._ply.shakeCamera(2f, 0.002f); // Gate shake
    }

    private void onGateMovementFinish(bool isReverse) {
        if(isReverse) {
            // Reset Elevator
            this._button.setButtonLocked(false);
            this._ply = null;
        } else {
            // TELEPORT
            if(this.itemSpot.hasItem()) {
                this.teleTo.itemSpot.deleteItem(); // Clear any item on the other elevator, just in case
                this.teleTo.itemSpot.placeItem(this.itemSpot.takeItem(), true);
            }

            this._teleport.teleport(this.teleTo.transform);
            // ----

            this._ply.shakeCamera(this.elevatorTime);

            if(this._elevatorMoveSnd != null) this._elevatorMoveSnd.Destroy();
            this._elevatorMoveSnd = SoundController.Instance.Play3DSound(this._audioClips[1], this.teleTo.transform);

            util_timer.simple(this.elevatorTime, () => {
                if(this._elevatorMoveSnd != null) this._elevatorMoveSnd.Destroy();
                this._ply.shakeCamera(2f, 0.003f); // Gate shake

                // Elevator stop
                SoundController.Instance.Play3DSound(this._audioClips[2], this.teleTo.transform);

                // Open both doors
                this.elevatorGate.reverse = true;
                this.elevatorGate.start();
                this.teleTo.elevatorGate.reverse = true;
                this.teleTo.elevatorGate.start();
                // ----
            });
        }
    }
}
