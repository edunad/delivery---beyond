
using UnityEngine;

[RequireComponent(typeof(entity_movement))]
public class entity_send_package : MonoBehaviour {

    public entity_button button;
    public entity_item_spot spot;

    #region PRIVATE
        private entity_movement _elevatorGate;
        private entity_box _box;
    #endregion

    public void Awake() {
        CoreController.Instance.OnGameStatusUpdated += this.onStatusChange;

        this._elevatorGate = GetComponent<entity_movement>();
        this._elevatorGate.reverse = false;

        this.button.OnUSE += onButtonPress;

        this.spot.OnItemDrop += this.enableElevator;
        this.spot.OnItemPickup += this.disableElevator;
        this.spot.setLocked(true);

        this._elevatorGate.OnMovementFinish += this.onGateMovementFinish;
    }

    private void onStatusChange(GAMEPLAY_STATUS oldStatus, GAMEPLAY_STATUS newStatus) {
        if(newStatus == GAMEPLAY_STATUS.COMPLETING) {
            this.spot.setLocked(false);
        } else {
            this.spot.setLocked(true);
            this.spot.deleteItem();
        }
    }

    private void onButtonPress(entity_player ply) {
        this.spot.setLocked(true);

        this._elevatorGate.reverse = false;
        this._elevatorGate.start();
    }

    private void enableElevator(entity_item itm) {
        entity_box box = itm.GetComponent<entity_box>();
        if(box == null) throw new System.Exception("Invalid item, missing entity_box");

        this._box = box;
        this.button.setButtonLocked(false);
    }

    private void disableElevator(entity_item itm) {
        this._box = null;
        this.button.setButtonLocked(true);
    }

    private void onGateMovementFinish(bool reverse) {
        if(reverse) return;
        if(this._box == null) throw new System.Exception("Missing box");

        GAME_REGIONS boxRegion = this._box.region;
        GAME_COUNTRIES clientCountry = CoreController.Instance.servingClient.getSetting<GAME_COUNTRIES>("country");
        bool isOK = CoreController.Instance.validateCountry(clientCountry, boxRegion);

        // TODO: Play elevator sound?

        util_timer.simple(2f, () => {
            this.spot.deleteItem();
            this._box = null;

            if(!isOK) CoreController.Instance.penalize("Wrong region set on item");
            CoreController.Instance.proccedEvent();

            this._elevatorGate.reverse = true;
            this._elevatorGate.start();
        });
    }
}
