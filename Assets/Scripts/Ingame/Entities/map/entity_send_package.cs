
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
public class entity_send_package : MonoBehaviour {

    public entity_button button;
    public entity_item_spot spot;

    private entity_movement _elevatorGate;
    private bool _isSending;

    public void Awake() {
        this._elevatorGate = GetComponent<entity_movement>();
        this._elevatorGate.reverse = false;

        this._isSending = false;

        this.spot.OnItemDrop += this.enableElevator;
        this.spot.OnItemPickup += this.disableElevator;

        this._elevatorGate.OnMovementFinish += this.onGateMovementFinish;
    }

    public void disableElevator(entity_item itm) {
        this.spot.locked = false;
        this.button.setButtonLocked(true);
    }

    public void enableElevator(entity_item itm) {
        this._isSending = false;
        this.button.setButtonLocked(false);
    }

    public void onButtonPress() {
        this.spot.locked = true;

        this._elevatorGate.reverse = false;
        this._elevatorGate.start();
    }

    private void onGateMovementFinish(bool reverse) {
        if(this._isSending || reverse) return;

        this._isSending = true;
        util_timer.UniqueSimple("elevator_reset", 2f, () => {
            this.spot.deleteItem();

            this._elevatorGate.reverse = true;
            this._elevatorGate.start();
        });
    }
}
